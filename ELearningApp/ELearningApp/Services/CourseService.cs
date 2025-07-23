using Microsoft.EntityFrameworkCore;
using ELearningApp.Data;
using ELearningApp.Models.Entities;
using ELearningApp.Models.Enums;
using ELearningApp.Models.DTOs;

namespace ELearningApp.Services
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
                return null; // Return null instead of throwing to prevent cascading failures
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
                _logger.LogInformation("Starting course creation for: {Title}", course.Title);
                
                // Ensure required fields are set
                course.CreatedAt = DateTime.UtcNow;
                course.UpdatedAt = DateTime.UtcNow;
                course.Status = CourseStatus.Draft;
                course.IsPublished = false;

                // Validate that InstructorId is not empty
                if (string.IsNullOrEmpty(course.InstructorId))
                {
                    throw new ArgumentException("InstructorId is required");
                }

                _logger.LogInformation("Validating instructor exists: {InstructorId}", course.InstructorId);

                // Check if the instructor exists
                var instructor = await _context.Users.FindAsync(course.InstructorId);
                if (instructor == null)
                {
                    throw new ArgumentException($"Instructor with ID {course.InstructorId} not found");
                }

                _logger.LogInformation("Instructor found: {InstructorName}", instructor.UserName);

                // Clear navigation properties to prevent EF tracking issues
                course.Instructor = null!;
                course.Category = null;

                _logger.LogInformation("Adding course to context");
                _context.Courses.Add(course);
                
                _logger.LogInformation("Saving changes to database");
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new course with ID {CourseId}", course.Id);
                return course;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course. Course data: Title: {Title}, InstructorId: {InstructorId}, CategoryId: {CategoryId}, Duration: {Duration}", 
                    course.Title, 
                    course.InstructorId, 
                    course.CategoryId, 
                    course.EstimatedDurationMinutes);
                
                // Log the inner exception details
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    _logger.LogError("Inner exception: {Message}", innerEx.Message);
                    innerEx = innerEx.InnerException;
                }
                
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
            double? minRating = null, int pageNumber = 1, int pageSize = 9)
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

        public async Task<int> GetFilteredCoursesCountAsync(int? categoryId = null, CourseLevel? level = null, double? minRating = null)
        {
            try
            {
                var query = _context.Courses
                    .Where(c => c.Status == CourseStatus.Published);

                if (categoryId.HasValue)
                    query = query.Where(c => c.CategoryId == categoryId.Value);

                if (level.HasValue)
                    query = query.Where(c => c.Level == level.Value);

                if (minRating.HasValue)
                    query = query.Where(c => c.AverageRating >= minRating.Value);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered courses count");
                return 0;
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
                    .Include(e => e.Course)
                    .ThenInclude(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == studentId);

                if (enrollment == null)
                    return 0;

                // Calculate progress based on completed lessons
                var totalLessons = enrollment.Course.Modules
                    .SelectMany(m => m.Lessons)
                    .Count();

                if (totalLessons == 0)
                    return 0;

                var completedLessons = await _context.LessonProgress
                    .Where(lp => lp.EnrollmentId == enrollment.Id && lp.IsCompleted)
                    .CountAsync();

                var progressPercentage = (double)completedLessons / totalLessons * 100;

                // Update the enrollment record with the calculated progress
                if (Math.Abs(enrollment.ProgressPercentage - progressPercentage) > 0.01)
                {
                    enrollment.ProgressPercentage = progressPercentage;
                    enrollment.LastAccessedDate = DateTime.UtcNow;
                    
                    // Check if course is completed
                    if (progressPercentage >= 100 && enrollment.CompletionDate == null)
                    {
                        enrollment.CompletionDate = DateTime.UtcNow;
                        enrollment.Status = EnrollmentStatus.Completed;
                    }
                    
                    await _context.SaveChangesAsync();
                }

                return progressPercentage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course progress for student {StudentId} in course {CourseId}", studentId, courseId);
                return 0;
            }
        }

        public async Task<bool> UpdateLessonProgressAsync(int lessonId, string studentId, bool isCompleted)
        {
            try
            {
                // First, get the enrollment for this student and course
                var enrollment = await _context.Enrollments
                    .Include(e => e.Course)
                    .ThenInclude(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                    .FirstOrDefaultAsync(e => e.UserId == studentId && 
                        e.Course.Modules.Any(m => m.Lessons.Any(l => l.Id == lessonId)));
                
                if (enrollment == null)
                {
                    _logger.LogWarning("No enrollment found for student {StudentId} and lesson {LessonId}", studentId, lessonId);
                    return false;
                }

                var progress = await _context.LessonProgress
                    .FirstOrDefaultAsync(lp => lp.LessonId == lessonId && lp.EnrollmentId == enrollment.Id);

                if (progress == null)
                {
                    progress = new LessonProgress
                    {
                        LessonId = lessonId,
                        EnrollmentId = enrollment.Id,
                        IsCompleted = isCompleted,
                        CompletedAt = isCompleted ? DateTime.UtcNow : null,
                        LastAccessedAt = DateTime.UtcNow,
                        ProgressPercentage = isCompleted ? 100.0 : 0.0
                    };
                    _context.LessonProgress.Add(progress);
                }
                else
                {
                    progress.IsCompleted = isCompleted;
                    progress.CompletedAt = isCompleted ? DateTime.UtcNow : progress.CompletedAt;
                    progress.LastAccessedAt = DateTime.UtcNow;
                    progress.ProgressPercentage = isCompleted ? 100.0 : progress.ProgressPercentage;
                }

                await _context.SaveChangesAsync();

                // Recalculate and update overall course progress
                var totalLessons = enrollment.Course.Modules
                    .SelectMany(m => m.Lessons)
                    .Count();

                if (totalLessons > 0)
                {
                    var completedLessons = await _context.LessonProgress
                        .Where(lp => lp.EnrollmentId == enrollment.Id && lp.IsCompleted)
                        .CountAsync();

                    var courseProgressPercentage = (double)completedLessons / totalLessons * 100;
                    
                    enrollment.ProgressPercentage = courseProgressPercentage;
                    enrollment.LastAccessedDate = DateTime.UtcNow;
                    
                    // Check if course is completed
                    if (courseProgressPercentage >= 100 && enrollment.CompletionDate == null)
                    {
                        enrollment.CompletionDate = DateTime.UtcNow;
                        enrollment.Status = EnrollmentStatus.Completed;
                    }
                    
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Updated lesson progress for lesson {LessonId}, student {StudentId}, completed: {IsCompleted}", 
                    lessonId, studentId, isCompleted);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson progress for lesson {LessonId}, student {StudentId}", lessonId, studentId);
                return false;
            }
        }

        // Add method to get lesson progress for a specific student
        public async Task<Dictionary<int, bool>> GetLessonProgressAsync(int courseId, string studentId)
        {
            try
            {
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == studentId);

                if (enrollment == null)
                    return new Dictionary<int, bool>();

                var progress = await _context.LessonProgress
                    .Where(lp => lp.EnrollmentId == enrollment.Id)
                    .ToDictionaryAsync(lp => lp.LessonId, lp => lp.IsCompleted);

                return progress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lesson progress for student {StudentId} in course {CourseId}", studentId, courseId);
                return new Dictionary<int, bool>();
            }
        }

        public Task<bool> MarkLessonCompletedAsync(int lessonId, string studentId)
        {
            return UpdateLessonProgressAsync(lessonId, studentId, true);
        }

        public async Task<IEnumerable<LessonProgress>> GetStudentProgressAsync(string studentId)
        {
            try
            {
                var progress = await _context.LessonProgress
                    .Include(lp => lp.Enrollment)
                    .ThenInclude(e => e.Course)
                    .Include(lp => lp.Lesson)
                    .ThenInclude(l => l.Module)
                    .Where(lp => lp.Enrollment.UserId == studentId)
                    .OrderBy(lp => lp.Lesson.Module.OrderIndex)
                    .ThenBy(lp => lp.Lesson.OrderIndex)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {ProgressCount} progress records for student {StudentId}", 
                    progress.Count, studentId);
                return progress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student progress for student {StudentId}", studentId);
                return Enumerable.Empty<LessonProgress>();
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

        #region Module Management

        public async Task<Module> CreateModuleAsync(Module module)
        {
            try
            {
                // Set default values
                module.CreatedAt = DateTime.UtcNow;
                module.UpdatedAt = DateTime.UtcNow;

                _context.Modules.Add(module);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created module {ModuleId} for course {CourseId}", 
                    module.Id, module.CourseId);
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating module for course {CourseId}", module.CourseId);
                throw;
            }
        }

        public async Task<Module> UpdateModuleAsync(Module module)
        {
            try
            {
                var existing = await _context.Modules.FindAsync(module.Id);
                if (existing == null)
                    throw new ArgumentException("Module not found");

                existing.Title = module.Title;
                existing.Description = module.Description;
                existing.OrderIndex = module.OrderIndex;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated module {ModuleId}", module.Id);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating module {ModuleId}", module.Id);
                throw;
            }
        }

        public async Task<bool> DeleteModuleAsync(int moduleId)
        {
            try
            {
                var module = await _context.Modules
                    .Include(m => m.Lessons)
                    .FirstOrDefaultAsync(m => m.Id == moduleId);

                if (module == null)
                    return false;

                // Delete all lessons in the module first
                if (module.Lessons?.Any() == true)
                {
                    _context.Lessons.RemoveRange(module.Lessons);
                }

                _context.Modules.Remove(module);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted module {ModuleId} and its lessons", moduleId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting module {ModuleId}", moduleId);
                return false;
            }
        }

        public async Task<Module?> GetModuleByIdAsync(int moduleId)
        {
            try
            {
                var module = await _context.Modules
                    .Include(m => m.Lessons)
                    .FirstOrDefaultAsync(m => m.Id == moduleId);

                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting module {ModuleId}", moduleId);
                return null;
            }
        }

        #endregion

        #region Lesson Management

        public async Task<Lesson> CreateLessonAsync(Lesson lesson)
        {
            try
            {
                // Set default values
                lesson.CreatedAt = DateTime.UtcNow;
                lesson.UpdatedAt = DateTime.UtcNow;

                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created lesson {LessonId} for module {ModuleId}", 
                    lesson.Id, lesson.ModuleId);
                return lesson;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lesson for module {ModuleId}", lesson.ModuleId);
                throw;
            }
        }

        public async Task<Lesson> UpdateLessonAsync(Lesson lesson)
        {
            try
            {
                var existing = await _context.Lessons.FindAsync(lesson.Id);
                if (existing == null)
                    throw new ArgumentException("Lesson not found");

                existing.Title = lesson.Title;
                existing.Description = lesson.Description;
                existing.Content = lesson.Content;
                existing.Type = lesson.Type;
                existing.OrderIndex = lesson.OrderIndex;
                existing.DurationMinutes = lesson.DurationMinutes;
                existing.IsFree = lesson.IsFree;
                existing.VideoUrl = lesson.VideoUrl;
                existing.ArticleUrl = lesson.ArticleUrl;
                existing.DownloadUrl = lesson.DownloadUrl;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated lesson {LessonId}", lesson.Id);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson {LessonId}", lesson.Id);
                throw;
            }
        }

        public async Task<bool> DeleteLessonAsync(int lessonId)
        {
            try
            {
                var lesson = await _context.Lessons.FindAsync(lessonId);
                if (lesson == null)
                    return false;

                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted lesson {LessonId}", lessonId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson {LessonId}", lessonId);
                return false;
            }
        }

        public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
        {
            try
            {
                var lesson = await _context.Lessons
                    .Include(l => l.Module)
                    .FirstOrDefaultAsync(l => l.Id == lessonId);

                return lesson;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lesson {LessonId}", lessonId);
                return null;
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
                return false;
            }
        }

        #endregion

        #region Course Announcements

        public async Task<List<CourseAnnouncement>> GetCourseAnnouncementsAsync(int courseId)
        {
            try
            {
                return await _context.CourseAnnouncements
                    .Include(ca => ca.Instructor)
                    .Where(ca => ca.CourseId == courseId && ca.IsPublished)
                    .OrderByDescending(ca => ca.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting announcements for course {CourseId}", courseId);
                return new List<CourseAnnouncement>();
            }
        }

        public async Task<CourseAnnouncement> CreateAnnouncementAsync(CourseAnnouncement announcement)
        {
            try
            {
                announcement.CreatedAt = DateTime.UtcNow;
                announcement.UpdatedAt = DateTime.UtcNow;

                _context.CourseAnnouncements.Add(announcement);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created announcement {AnnouncementId} for course {CourseId}", 
                    announcement.Id, announcement.CourseId);
                return announcement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating announcement for course {CourseId}", announcement.CourseId);
                throw;
            }
        }

        public async Task<CourseAnnouncement> UpdateAnnouncementAsync(CourseAnnouncement announcement)
        {
            try
            {
                var existing = await _context.CourseAnnouncements.FindAsync(announcement.Id);
                if (existing == null)
                    throw new ArgumentException("Announcement not found");

                existing.Title = announcement.Title;
                existing.Content = announcement.Content;
                existing.IsImportant = announcement.IsImportant;
                existing.IsPublished = announcement.IsPublished;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated announcement {AnnouncementId}", announcement.Id);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating announcement {AnnouncementId}", announcement.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAnnouncementAsync(int announcementId)
        {
            try
            {
                var announcement = await _context.CourseAnnouncements.FindAsync(announcementId);
                if (announcement == null)
                    return false;

                _context.CourseAnnouncements.Remove(announcement);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted announcement {AnnouncementId}", announcementId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting announcement {AnnouncementId}", announcementId);
                return false;
            }
        }

        #endregion

        #region Course Reviews

        public async Task<List<CourseReview>> GetCourseReviewsAsync(int courseId)
        {
            try
            {
                var reviews = await _context.CourseReviews
                    .Include(cr => cr.Student)
                    .Where(cr => cr.CourseId == courseId)
                    .OrderByDescending(cr => cr.CreatedAt)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {ReviewCount} reviews for course {CourseId}", reviews.Count, courseId);
                return reviews;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for course {CourseId}", courseId);
                return new List<CourseReview>();
            }
        }

        #endregion
    }
}