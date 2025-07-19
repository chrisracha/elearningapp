using ELearningApp.Models.Entities;
using ELearningApp.Models.Enums;
using ELearningApp.Models.DTOs;

namespace ELearningApp.Services
{
    public interface ICourseService
    {
        // Course CRUD Operations
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course?> GetCourseWithDetailsAsync(int id);
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId);
        Task<IEnumerable<Course>> GetCoursesByInstructorAsync(string instructorId);
        Task<IEnumerable<Course>> GetFeaturedCoursesAsync();
        Task<IEnumerable<Course>> GetPublishedCoursesAsync();
        Task<Course> CreateCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int id);

        // Course Search and Filtering
        Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm);
        Task<IEnumerable<Course>> FilterCoursesAsync(
            int? categoryId = null,
            CourseLevel? level = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            double? minRating = null,
            int pageNumber = 1,
            int pageSize = 12
        );

        // Course Statistics
        Task<int> GetTotalCoursesCountAsync();
        Task<int> GetPublishedCoursesCountAsync();
        Task<int> GetCoursesByCategoryCountAsync(int categoryId);
        Task<IEnumerable<Course>> GetTopRatedCoursesAsync(int count = 10);
        Task<IEnumerable<Course>> GetMostPopularCoursesAsync(int count = 10);

        // Course Management
        Task<bool> PublishCourseAsync(int courseId);
        Task<bool> UnpublishCourseAsync(int courseId);
        Task<bool> UpdateCourseStatsAsync(int courseId);
        Task<bool> IncrementViewCountAsync(int courseId);

        // Enrollment Operations
        Task<bool> EnrollStudentAsync(int courseId, string studentId, decimal paidAmount);
        Task<bool> UnenrollStudentAsync(int courseId, string studentId);
        Task<bool> IsStudentEnrolledAsync(int courseId, string studentId);
        Task<Enrollment?> GetEnrollmentAsync(int courseId, string studentId);
        Task<IEnumerable<Enrollment>> GetStudentEnrollmentsAsync(string studentId);
        Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(int courseId);

        // Progress Tracking
        Task<bool> UpdateLessonProgressAsync(int enrollmentId, int lessonId, bool isCompleted, int timeSpentMinutes);
        Task<double> GetCourseProgressAsync(int courseId, string studentId);
        Task<IEnumerable<LessonProgress>> GetLessonProgressAsync(int enrollmentId);

        // Reviews and Ratings
        Task<CourseReview> AddReviewAsync(int courseId, string studentId, int rating, string? reviewText);
        Task<CourseReview> UpdateReviewAsync(int reviewId, int rating, string? reviewText);
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<IEnumerable<CourseReview>> GetCourseReviewsAsync(int courseId);
        Task<double> GetCourseAverageRatingAsync(int courseId);
    }
} 