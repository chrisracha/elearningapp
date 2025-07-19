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
                    .Where(c => c.IsFeatured && c.Status == CourseStatus.Published)
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

        public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Where(c => c.Status == CourseStatus.Published)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published courses");
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

        #endregion

        #region Course Search and Filtering

        public async Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetPublishedCoursesAsync();

                var normalizedSearch = searchTerm.ToLower();

                return await _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Where(c => c.Status == CourseStatus.Published && 
                               (c.Title.ToLower().Contains(normalizedSearch) ||
                                c.ShortDescription.ToLower().Contains(normalizedSearch) ||
                                c.LongDescription!.ToLower().Contains(normalizedSearch) ||
                                c.Category.Name.ToLower().Contains(normalizedSearch)))
                    .OrderByDescending(c => c.AverageRating)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching courses with term {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Course>> FilterCoursesAsync(
            int? categoryId = null,
            CourseLevel? level = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            double? minRating = null,
            int pageNumber = 1,
            int pageSize = 12)
        {
            try
            {
                var query = _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Where(c => c.Status == CourseStatus.Published);

                if (categoryId.HasValue)
                    query = query.Where(c => c.CategoryId == categoryId.Value);

                if (level.HasValue)
                    query = query.Where(c => c.Level == level.Value);

                if (minPrice.HasValue)
                    query = query.Where(c => c.Price >= minPrice.Value);

                if (maxPrice.HasValue)
                    query = query.Where(c => c.Price <= maxPrice.Value);

                if (minRating.HasValue)
                    query = query.Where(c => c.AverageRating >= minRating.Value);

                return await query
                    .OrderByDescending(c => c.AverageRating)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering courses");
                throw;
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
                course.PublishedAt = DateTime.UtcNow;
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
                if (course == null)
                    return false;

                course.ViewCount++;
                course.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing view count for course {CourseId}", courseId);
                throw;
            }
        }

        #endregion

        #region Enrollment Operations

        public async Task<bool> EnrollStudentAsync(int courseId, string studentId, decimal paidAmount)
        {
            try
            {
                // Check if already enrolled
                var existingEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

                if (existingEnrollment != null)
                {
                    if (existingEnrollment.Status == EnrollmentStatus.Active)
                        return false; // Already enrolled

                    // Reactivate enrollment
                    existingEnrollment.Status = EnrollmentStatus.Active;
                    existingEnrollment.EnrolledAt = DateTime.UtcNow;
                    existingEnrollment.PaidAmount = paidAmount;
                }
                else
                {
                    var enrollment = new Enrollment
                    {
                        CourseId = courseId,
                        StudentId = studentId,
                        Status = EnrollmentStatus.Active,
                        EnrolledAt = DateTime.UtcNow,
                        LastAccessedAt = DateTime.UtcNow,
                        PaidAmount = paidAmount,
                        ProgressPercentage = 0
                    };

                    _context.Enrollments.Add(enrollment);
                }

                await _context.SaveChangesAsync();
                await UpdateCourseStatsAsync(courseId);

                _logger.LogInformation("Enrolled student {StudentId} in course {CourseId}", studentId, courseId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling student {StudentId} in course {CourseId}", studentId, courseId);
                throw;
            }
        }

        public async Task<bool> UnenrollStudentAsync(int courseId, string studentId)
        {
            try
            {
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

                if (enrollment == null)
                    return false;

                enrollment.Status = EnrollmentStatus.Dropped;
                await _context.SaveChangesAsync();
                await UpdateCourseStatsAsync(courseId);

                _logger.LogInformation("Unenrolled student {StudentId} from course {CourseId}", studentId, courseId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unenrolling student {StudentId} from course {CourseId}", studentId, courseId);
                throw;
            }
        }

        public async Task<bool> IsStudentEnrolledAsync(int courseId, string studentId)
        {
            try
            {
                return await _context.Enrollments
                    .AnyAsync(e => e.CourseId == courseId && 
                                  e.StudentId == studentId && 
                                  e.Status == EnrollmentStatus.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking enrollment status for student {StudentId} in course {CourseId}", studentId, courseId);
                throw;
            }
        }

        public async Task<Enrollment?> GetEnrollmentAsync(int courseId, string studentId)
        {
            try
            {
                return await _context.Enrollments
                    .Include(e => e.Course)
                    .Include(e => e.Student)
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollment for student {StudentId} in course {CourseId}", studentId, courseId);
                throw;
            }
        }

        public async Task<IEnumerable<Enrollment>> GetStudentEnrollmentsAsync(string studentId)
        {
            try
            {
                return await _context.Enrollments
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Category)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                    .Where(e => e.StudentId == studentId)
                    .OrderByDescending(e => e.EnrolledAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollments for student {StudentId}", studentId);
                throw;
            }
        }

        public async Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(int courseId)
        {
            try
            {
                return await _context.Enrollments
                    .Include(e => e.Student)
                    .Where(e => e.CourseId == courseId)
                    .OrderByDescending(e => e.EnrolledAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollments for course {CourseId}", courseId);
                throw;
            }
        }

        #endregion

        #region Progress Tracking

        public async Task<bool> UpdateLessonProgressAsync(int enrollmentId, int lessonId, bool isCompleted, int timeSpentMinutes)
        {
            try
            {
                var progress = await _context.LessonProgress
                    .FirstOrDefaultAsync(lp => lp.EnrollmentId == enrollmentId && lp.LessonId == lessonId);

                if (progress == null)
                {
                    progress = new LessonProgress
                    {
                        EnrollmentId = enrollmentId,
                        LessonId = lessonId,
                        IsCompleted = isCompleted,
                        CompletedAt = isCompleted ? DateTime.UtcNow : null,
                        LastAccessedAt = DateTime.UtcNow,
                        TimeSpentMinutes = timeSpentMinutes,
                        ProgressPercentage = isCompleted ? 100 : 0
                    };

                    _context.LessonProgress.Add(progress);
                }
                else
                {
                    progress.IsCompleted = isCompleted;
                    progress.CompletedAt = isCompleted ? DateTime.UtcNow : progress.CompletedAt;
                    progress.LastAccessedAt = DateTime.UtcNow;
                    progress.TimeSpentMinutes += timeSpentMinutes;
                    progress.ProgressPercentage = isCompleted ? 100 : progress.ProgressPercentage;
                }

                await _context.SaveChangesAsync();

                // Update enrollment progress
                var enrollment = await _context.Enrollments
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Modules)
                            .ThenInclude(m => m.Lessons)
                    .FirstOrDefaultAsync(e => e.Id == enrollmentId);

                if (enrollment != null)
                {
                    var totalLessons = enrollment.Course.Modules.SelectMany(m => m.Lessons).Count();
                    var completedLessons = await _context.LessonProgress
                        .CountAsync(lp => lp.EnrollmentId == enrollmentId && lp.IsCompleted);

                    enrollment.ProgressPercentage = totalLessons > 0 ? (double)completedLessons / totalLessons * 100 : 0;
                    enrollment.LastAccessedAt = DateTime.UtcNow;

                    if (enrollment.ProgressPercentage >= 100 && enrollment.Status == EnrollmentStatus.Active)
                    {
                        enrollment.Status = EnrollmentStatus.Completed;
                        enrollment.CompletedAt = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson progress for enrollment {EnrollmentId}, lesson {LessonId}", enrollmentId, lessonId);
                throw;
            }
        }

        public async Task<double> GetCourseProgressAsync(int courseId, string studentId)
        {
            try
            {
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

                return enrollment?.ProgressPercentage ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course progress for student {StudentId} in course {CourseId}", studentId, courseId);
                throw;
            }
        }

        public async Task<IEnumerable<LessonProgress>> GetLessonProgressAsync(int enrollmentId)
        {
            try
            {
                return await _context.LessonProgress
                    .Include(lp => lp.Lesson)
                    .Where(lp => lp.EnrollmentId == enrollmentId)
                    .OrderBy(lp => lp.Lesson.OrderIndex)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lesson progress for enrollment {EnrollmentId}", enrollmentId);
                throw;
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
                throw;
            }
        }

        #endregion
    }
} 