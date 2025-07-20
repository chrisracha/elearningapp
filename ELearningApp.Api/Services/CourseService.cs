using Microsoft.EntityFrameworkCore;
using ELearningApp.Api.Data;
using ELearningApp.Api.Models.Entities;
using ELearningApp.Api.Models.Enums;
using ELearningApp.Api.Models.DTOs;

namespace ELearningApp.Api.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CourseService> _logger;

        public CourseService(ApplicationDbContext context, ILogger<CourseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Course CRUD Operations

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course by ID {CourseId}", id);
                throw;
            }
        }

        public async Task<Course?> GetCourseWithDetailsAsync(int id)
        {
            try
            {
                return await _context.Courses
                    .AsSplitQuery() // Prevent cartesian explosion
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Include(c => c.Modules.OrderBy(m => m.OrderIndex))
                        .ThenInclude(m => m.Lessons.OrderBy(l => l.OrderIndex))
                    .Include(c => c.Reviews.Take(10))
                        .ThenInclude(r => r.Student)
                    .Include(c => c.CourseTags)
                        .ThenInclude(ct => ct.Tag)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course with details for ID {CourseId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all courses");
                throw;
            }
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId)
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Where(c => c.CategoryId == categoryId && c.Status == CourseStatus.Published)
                    .OrderByDescending(c => c.AverageRating)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses by category {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<IEnumerable<Course>> GetCoursesByInstructorAsync(string instructorId)
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Where(c => c.InstructorId == instructorId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses by instructor {InstructorId}", instructorId);
                throw;
            }
        }

        public async Task<IEnumerable<Course>> GetFeaturedCoursesAsync()
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Where(c => c.Status == CourseStatus.Published)
                    .OrderByDescending(c => c.AverageRating)
                    .Take(8)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured courses");
                throw;
            }
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            try
            {
                course.CreatedAt = DateTime.UtcNow;
                course.UpdatedAt = DateTime.UtcNow;
                course.Status = CourseStatus.Draft;

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new course with ID {CourseId}", course.Id);
                return course;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                throw;
            }
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            try
            {
                course.UpdatedAt = DateTime.UtcNow;
                
                _context.Courses.Update(course);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated course with ID {CourseId}", course.Id);
                return course;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course {CourseId}", course.Id);
                throw;
            }
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            // Basic delete method - kept for backward compatibility
            // Use DeleteCourseWithValidationAsync for enhanced functionality
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return false;

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted course with ID {CourseId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course {CourseId}", id);
                throw;
            }
        }

        public async Task<(bool Success, string Message)> DeleteCourseWithValidationAsync(int courseId, string instructorId)
        {
            try
            {
                // Validate course exists and instructor has permission
                var course = await _context.Courses
                    .Include(c => c.Enrollments)
                    .Include(c => c.Modules)
                        .ThenInclude(m => m.Lessons)
                            .ThenInclude(l => l.LessonProgress)
                    .Include(c => c.Reviews)
                    .Include(c => c.CourseTags)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                    return (false, "Course not found.");

                if (course.InstructorId != instructorId)
                    return (false, "You don't have permission to delete this course.");

                // Check business rules
                var activeEnrollments = course.Enrollments.Count(e => e.Status == EnrollmentStatus.Active);
                var hasContent = course.Modules.Any();

                // Business rules for deletion
                if (course.Status == CourseStatus.Published && activeEnrollments > 0)
                {
                    return (false, $"Cannot delete published course with {activeEnrollments} active enrollment(s). Consider archiving instead.");
                }

                if (course.Status == CourseStatus.Published && course.Enrollments.Any())
                {
                    return (false, "Cannot delete published course with enrollment history. Consider archiving instead.");
                }

                // Proceed with deletion based on course state
                if (activeEnrollments == 0 && !course.Enrollments.Any())
                {
                    // Safe to hard delete - no enrollment history
                    await HardDeleteCourseAsync(course);
                    _logger.LogInformation("Hard deleted course {CourseId} by instructor {InstructorId}", courseId, instructorId);
                    return (true, "Course deleted successfully.");
                }
                else
                {
                    // Soft delete - archive the course
                    await SoftDeleteCourseAsync(course);
                    _logger.LogInformation("Soft deleted (archived) course {CourseId} by instructor {InstructorId}", courseId, instructorId);
                    return (true, "Course archived successfully. Students will no longer be able to access it.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course {CourseId} by instructor {InstructorId}", courseId, instructorId);
                return (false, "An error occurred while deleting the course. Please try again.");
            }
        }

        public async Task<bool> CanDeleteCourseAsync(int courseId, string instructorId)
        {
            try
            {
                var course = await _context.Courses
                    .Include(c => c.Enrollments)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null || course.InstructorId != instructorId)
                    return false;

                // Check if course can be deleted (same business rules as delete)
                var activeEnrollments = course.Enrollments.Count(e => e.Status == EnrollmentStatus.Active);
                
                return !(course.Status == CourseStatus.Published && activeEnrollments > 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if course {CourseId} can be deleted", courseId);
                return false;
            }
        }

        public async Task<(bool HasEnrollments, int EnrollmentCount)> GetCourseDeletionInfoAsync(int courseId)
        {
            try
            {
                var enrollmentCount = await _context.Enrollments
                    .CountAsync(e => e.CourseId == courseId);

                return (enrollmentCount > 0, enrollmentCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting deletion info for course {CourseId}", courseId);
                return (false, 0);
            }
        }

        private async Task HardDeleteCourseAsync(Course course)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete in correct order to avoid foreign key constraints
                
                // 1. Delete lesson progress (if any)
                var lessonIds = course.Modules.SelectMany(m => m.Lessons).Select(l => l.Id).ToList();
                if (lessonIds.Any())
                {
                    var lessonProgress = await _context.LessonProgress
                        .Where(lp => lessonIds.Contains(lp.LessonId))
                        .ToListAsync();
                    _context.LessonProgress.RemoveRange(lessonProgress);
                }

                // 2. Delete lessons
                var lessons = course.Modules.SelectMany(m => m.Lessons).ToList();
                if (lessons.Any())
                {
                    _context.Lessons.RemoveRange(lessons);
                }

                // 3. Delete modules
                if (course.Modules.Any())
                {
                    _context.Modules.RemoveRange(course.Modules);
                }

                // 4. Delete course tags
                if (course.CourseTags.Any())
                {
                    _context.CourseTags.RemoveRange(course.CourseTags);
                }

                // 5. Delete reviews
                if (course.Reviews.Any())
                {
                    _context.CourseReviews.RemoveRange(course.Reviews);
                }

                // 6. Delete enrollments (should be none for hard delete)
                if (course.Enrollments.Any())
                {
                    _context.Enrollments.RemoveRange(course.Enrollments);
                }

                // 7. Finally delete the course
                _context.Courses.Remove(course);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task SoftDeleteCourseAsync(Course course)
        {
            // Archive the course instead of deleting
            course.Status = CourseStatus.Archived;
            course.UpdatedAt = DateTime.UtcNow;

            // Optionally, you could add an "IsDeleted" flag and "DeletedAt" timestamp
            // if you want to track soft deletes explicitly

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Course Discovery and Catalog

        public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.Instructor)
                    .Include(c => c.Category)
                    .Where(c => c.Status == CourseStatus.Published)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published courses");
                return Enumerable.Empty<Course>();
            }
        }

        public async Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetPublishedCoursesAsync();

                var searchTermLower = searchTerm.ToLowerInvariant();

                return await _context.Courses
                    .Include(c => c.Instructor)
                    .Include(c => c.Category)
                    .Where(c => c.Status == CourseStatus.Published &&
                               (c.Title.ToLower().Contains(searchTermLower) || 
                                c.ShortDescription.ToLower().Contains(searchTermLower) ||
                                (c.LongDescription != null && c.LongDescription.ToLower().Contains(searchTermLower)) ||
                                (c.Instructor != null && (
                                    (c.Instructor.FirstName != null && c.Instructor.FirstName.ToLower().Contains(searchTermLower)) ||
                                    (c.Instructor.LastName != null && c.Instructor.LastName.ToLower().Contains(searchTermLower)) ||
                                    (c.Instructor.FirstName + " " + c.Instructor.LastName).ToLower().Contains(searchTermLower)
                                ))))
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching courses with term: {SearchTerm}", searchTerm);
                return Enumerable.Empty<Course>();
            }
        }

        public async Task<IEnumerable<Course>> FilterCoursesAsync(int? categoryId = null, CourseLevel? level = null, 
            double? minRating = null, int pageNumber = 1, int pageSize = 12)
        {
            try
            {
                var query = _context.Courses
                    .Include(c => c.Instructor)
                    .Include(c => c.Category)
                    .Where(c => c.Status == CourseStatus.Published);

                if (categoryId.HasValue)
                    query = query.Where(c => c.CategoryId == categoryId.Value);

                if (level.HasValue)
                    query = query.Where(c => c.Level == level.Value);

                if (minRating.HasValue)
                    query = query.Where(c => c.AverageRating >= minRating.Value);

                return await query
                    .OrderByDescending(c => c.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering courses");
                return Enumerable.Empty<Course>();
            }
        }

        #endregion

        #region Course Statistics

        public async Task<int> GetTotalCoursesCountAsync()
        {
            try
            {
                return await _context.Courses.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total courses count");
                throw;
            }
        }

        public async Task<int> GetPublishedCoursesCountAsync()
        {
            try
            {
                return await _context.Courses
                    .CountAsync(c => c.Status == CourseStatus.Published);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published courses count");
                throw;
            }
        }

        public async Task<int> GetCoursesByCategoryCountAsync(int categoryId)
        {
            try
            {
                return await _context.Courses
                    .CountAsync(c => c.CategoryId == categoryId && c.Status == CourseStatus.Published);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses count by category {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<IEnumerable<Course>> GetTopRatedCoursesAsync(int count = 10)
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Where(c => c.Status == CourseStatus.Published && c.ReviewCount > 0)
                    .OrderByDescending(c => c.AverageRating)
                    .ThenByDescending(c => c.ReviewCount)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top rated courses");
                throw;
            }
        }

        public async Task<IEnumerable<Course>> GetMostPopularCoursesAsync(int count = 10)
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Where(c => c.Status == CourseStatus.Published)
                    .OrderByDescending(c => c.EnrollmentCount)
                    .ThenByDescending(c => c.ViewCount)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting most popular courses");
                throw;
            }
        }

        #endregion

        #region Course Management

        public async Task<bool> PublishCourseAsync(int courseId)
        {
            try
            {
                var course = await _context.Courses.FindAsync(courseId);
                if (course == null)
                    return false;

                course.Status = CourseStatus.Published;
                course.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Published course with ID {CourseId}", courseId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing course {CourseId}", courseId);
                throw;
            }
        }

        public async Task<bool> UnpublishCourseAsync(int courseId)
        {
            try
            {
                var course = await _context.Courses.FindAsync(courseId);
                if (course == null)
                    return false;

                course.Status = CourseStatus.Draft;
                course.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Unpublished course with ID {CourseId}", courseId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing course {CourseId}", courseId);
                throw;
            }
        }

        public async Task<bool> UpdateCourseStatsAsync(int courseId)
        {
            try
            {
                var course = await _context.Courses
                    .Include(c => c.Reviews)
                    .Include(c => c.Enrollments)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                    return false;

                // Update enrollment count
                course.EnrollmentCount = course.Enrollments.Count(e => e.Status == EnrollmentStatus.Active);

                // Update review stats
                if (course.Reviews.Any())
                {
                    course.AverageRating = course.Reviews.Average(r => r.Rating);
                    course.ReviewCount = course.Reviews.Count;
                }
                else
                {
                    course.AverageRating = 0;
                    course.ReviewCount = 0;
                }

                course.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated stats for course with ID {CourseId}", courseId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course stats {CourseId}", courseId);
                throw;
            }
        }

        public async Task<bool> IncrementViewCountAsync(int courseId)
        {
            try
            {
                var course = await _context.Courses.FindAsync(courseId);
                if (course != null)
                {
                    course.ViewCount++;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing view count for course {CourseId}", courseId);
                return false;
            }
        }

        #endregion

        #region Enrollment Management

        public async Task<bool> EnrollStudentAsync(int courseId, string studentId)
        {
            try
            {
                // Check if already enrolled
                if (await IsStudentEnrolledAsync(courseId, studentId))
                    return false;

                var enrollment = new Enrollment
                {
                    CourseId = courseId,
                    UserId = studentId,
                    EnrollmentDate = DateTime.UtcNow,
                    Status = EnrollmentStatus.Active
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Student {StudentId} enrolled in course {CourseId}", studentId, courseId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling student {StudentId} in course {CourseId}", studentId, courseId);
                return false;
            }
        }

        public async Task<bool> UnenrollStudentAsync(int courseId, string studentId)
        {
            try
            {
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == studentId);

                if (enrollment == null)
                    return false;

                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Student {StudentId} unenrolled from course {CourseId}", studentId, courseId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unenrolling student {StudentId} from course {CourseId}", studentId, courseId);
                return false;
            }
        }

        public async Task<bool> IsStudentEnrolledAsync(int courseId, string studentId)
        {
            try
            {
                return await _context.Enrollments
                    .AnyAsync(e => e.CourseId == courseId && e.UserId == studentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking enrollment for student {StudentId} in course {CourseId}", studentId, courseId);
                return false;
            }
        }

        public async Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(string userId)
        {
            try
            {
                return await _context.Enrollments
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Category)
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.LastAccessedDate ?? e.EnrollmentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollments for user {UserId}", userId);
                return Enumerable.Empty<Enrollment>();
            }
        }

        public async Task<Enrollment?> GetEnrollmentAsync(int courseId, string studentId)
        {
            try
            {
                return await _context.Enrollments
                    .Include(e => e.Course)
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == studentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollment for student {StudentId} in course {CourseId}", studentId, courseId);
                return null;
            }
        }

        #endregion

        #region Progress Tracking

        public async Task<double> GetCourseProgressAsync(int courseId, string studentId)
        {
            try
            {
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == studentId);

                return enrollment?.ProgressPercentage ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course progress for student {StudentId} in course {CourseId}", studentId, courseId);
                return 0;
            }
        }

        public Task<bool> UpdateLessonProgressAsync(int lessonId, string studentId, bool isCompleted)
        {
            try
            {
                // TODO: Implement lesson progress tracking
                // This will be implemented in Phase 2
                _logger.LogInformation("Lesson progress update requested for lesson {LessonId}, student {StudentId}, completed: {IsCompleted}", 
                    lessonId, studentId, isCompleted);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson progress for lesson {LessonId}, student {StudentId}", lessonId, studentId);
                return Task.FromResult(false);
            }
        }

        public Task<bool> MarkLessonCompletedAsync(int lessonId, string studentId)
        {
            return UpdateLessonProgressAsync(lessonId, studentId, true);
        }

        public Task<IEnumerable<LessonProgress>> GetStudentProgressAsync(string studentId)
        {
            try
            {
                // TODO: Implement lesson progress retrieval
                // This will be implemented in Phase 2
                _logger.LogInformation("Student progress requested for student {StudentId}", studentId);
                return Task.FromResult(Enumerable.Empty<LessonProgress>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student progress for student {StudentId}", studentId);
                return Task.FromResult(Enumerable.Empty<LessonProgress>());
            }
        }

        #endregion

        #region Course Statistics

        public async Task<bool> IncrementCourseViewAsync(int courseId)
        {
            try
            {
                var course = await _context.Courses.FindAsync(courseId);
                if (course != null)
                {
                    course.ViewCount++;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing view count for course {CourseId}", courseId);
                return false;
            }
        }

        public async Task<int> GetCourseEnrollmentCountAsync(int courseId)
        {
            try
            {
                return await _context.Enrollments
                    .CountAsync(e => e.CourseId == courseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollment count for course {CourseId}", courseId);
                return 0;
            }
        }

        public async Task<double> GetCourseAverageRatingAsync(int courseId)
        {
            try
            {
                var reviews = await _context.CourseReviews
                    .Where(r => r.CourseId == courseId)
                    .ToListAsync();

                return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average rating for course {CourseId}", courseId);
                return 0;
            }
        }

        #endregion

        #region Reviews and Ratings

        public async Task<CourseReview> AddReviewAsync(int courseId, string studentId, int rating, string? reviewText)
        {
            try
            {
                // Check if review already exists
                var existingReview = await _context.CourseReviews
                    .FirstOrDefaultAsync(r => r.CourseId == courseId && r.StudentId == studentId);

                if (existingReview != null)
                    throw new InvalidOperationException("Student has already reviewed this course");

                // Verify student is enrolled
                var isEnrolled = await IsStudentEnrolledAsync(courseId, studentId);
                
                var review = new CourseReview
                {
                    CourseId = courseId,
                    StudentId = studentId,
                    Rating = rating,
                    ReviewText = reviewText,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsVerifiedPurchase = isEnrolled
                };

                _context.CourseReviews.Add(review);
                await _context.SaveChangesAsync();
                await UpdateCourseStatsAsync(courseId);

                _logger.LogInformation("Added review for course {CourseId} by student {StudentId}", courseId, studentId);
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review for course {CourseId} by student {StudentId}", courseId, studentId);
                throw;
            }
        }

        public async Task<CourseReview> UpdateReviewAsync(int reviewId, int rating, string? reviewText)
        {
            try
            {
                var review = await _context.CourseReviews.FindAsync(reviewId);
                if (review == null)
                    throw new ArgumentException("Review not found");

                review.Rating = rating;
                review.ReviewText = reviewText;
                review.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await UpdateCourseStatsAsync(review.CourseId);

                _logger.LogInformation("Updated review {ReviewId}", reviewId);
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {ReviewId}", reviewId);
                throw;
            }
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            try
            {
                var review = await _context.CourseReviews.FindAsync(reviewId);
                if (review == null)
                    return false;

                var courseId = review.CourseId;
                _context.CourseReviews.Remove(review);
                await _context.SaveChangesAsync();
                await UpdateCourseStatsAsync(courseId);

                _logger.LogInformation("Deleted review {ReviewId}", reviewId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", reviewId);
                throw;
            }
        }

        public async Task<IEnumerable<CourseReview>> GetCourseReviewsAsync(int courseId)
        {
            try
            {
                return await _context.CourseReviews
                    .Include(r => r.Student)
                    .Where(r => r.CourseId == courseId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for course {CourseId}", courseId);
                throw;
            }
        }

        #endregion
    }
}