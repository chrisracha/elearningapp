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

        // Module Management
        Task<Module> CreateModuleAsync(Module module);
        Task<Module> UpdateModuleAsync(Module module);
        Task<bool> DeleteModuleAsync(int moduleId);
        Task<Module?> GetModuleByIdAsync(int moduleId);

        // Lesson Management
        Task<Lesson> CreateLessonAsync(Lesson lesson);
        Task<Lesson> UpdateLessonAsync(Lesson lesson);
        Task<bool> DeleteLessonAsync(int lessonId);
        Task<Lesson?> GetLessonByIdAsync(int lessonId);

        // Course Search and Filtering
        Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm);
        Task<IEnumerable<Course>> FilterCoursesAsync(
            int? categoryId = null,
            CourseLevel? level = null,
            double? minRating = null,
            int pageNumber = 1,
            int pageSize = 9
        );
        Task<int> GetFilteredCoursesCountAsync(int? categoryId = null, CourseLevel? level = null, double? minRating = null);

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

        // Enhanced Delete Operations
        Task<(bool Success, string Message)> DeleteCourseWithValidationAsync(int courseId, string instructorId);
        Task<bool> CanDeleteCourseAsync(int courseId, string instructorId);
        Task<(bool HasEnrollments, int EnrollmentCount)> GetCourseDeletionInfoAsync(int courseId);

        // Enrollment Management
        Task<bool> EnrollStudentAsync(int courseId, string studentId);
        Task<bool> UnenrollStudentAsync(int courseId, string studentId);
        Task<bool> IsStudentEnrolledAsync(int courseId, string studentId);
        Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(string userId);
        Task<Enrollment?> GetEnrollmentAsync(int courseId, string studentId);

        // Progress Tracking
        Task<double> GetCourseProgressAsync(int courseId, string studentId);
        Task<bool> UpdateLessonProgressAsync(int lessonId, string studentId, bool isCompleted);
        Task<bool> MarkLessonCompletedAsync(int lessonId, string studentId);
        Task<IEnumerable<LessonProgress>> GetStudentProgressAsync(string studentId);

        // Course Statistics
        Task<bool> IncrementCourseViewAsync(int courseId);
        Task<int> GetCourseEnrollmentCountAsync(int courseId);
        Task<double> GetCourseAverageRatingAsync(int courseId);

        // Course Announcements
        Task<List<CourseAnnouncement>> GetCourseAnnouncementsAsync(int courseId);
        Task<CourseAnnouncement> CreateAnnouncementAsync(CourseAnnouncement announcement);
        Task<CourseAnnouncement> UpdateAnnouncementAsync(CourseAnnouncement announcement);
        Task<bool> DeleteAnnouncementAsync(int announcementId);

        // Course Reviews
        Task<List<CourseReview>> GetCourseReviewsAsync(int courseId);
        Task<CourseReview> AddReviewAsync(int courseId, string studentId, int rating, string? reviewText);
        Task<CourseReview> UpdateReviewAsync(int reviewId, int rating, string? reviewText);
        Task<bool> DeleteReviewAsync(int reviewId);
    }
}