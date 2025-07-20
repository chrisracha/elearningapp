using System.Net.Http.Json;
using ELearningApp.Wasm.Wasm.Models.Entities;
using ELearningApp.Wasm.Wasm.Models.Enums;
using ELearningApp.Wasm.Wasm.Models.DTOs;

namespace ELearningApp.Wasm.Wasm.Services
{
    public class CourseService : ICourseService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CourseService> _logger;

        public CourseService(HttpClient httpClient, ILogger<CourseService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        #region Course CRUD Operations

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Course>($"api/courses/{id}");
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
                return await _httpClient.GetFromJsonAsync<Course>($"api/courses/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course with details by ID {CourseId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CourseDisplayDto>> GetAllCoursesAsync()
        {
            try
            {
                var courses = await _httpClient.GetFromJsonAsync<IEnumerable<CourseDisplayDto>>("api/courses");
                return courses ?? Enumerable.Empty<CourseDisplayDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all courses");
                throw;
            }
        }

        public async Task<IEnumerable<CourseDisplayDto>> GetFeaturedCoursesAsync()
        {
            try
            {
                var courses = await _httpClient.GetFromJsonAsync<IEnumerable<CourseDisplayDto>>("api/courses/featured");
                return courses ?? Enumerable.Empty<CourseDisplayDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured courses");
                throw;
            }
        }

        public async Task<IEnumerable<CourseDisplayDto>> GetPopularCoursesAsync()
        {
            try
            {
                var courses = await _httpClient.GetFromJsonAsync<IEnumerable<CourseDisplayDto>>("api/courses/popular");
                return courses ?? Enumerable.Empty<CourseDisplayDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular courses");
                throw;
            }
        }

        public async Task<IEnumerable<CourseDisplayDto>> GetCoursesByCategoryAsync(int categoryId)
        {
            try
            {
                var courses = await _httpClient.GetFromJsonAsync<IEnumerable<CourseDisplayDto>>($"api/courses/category/{categoryId}");
                return courses ?? Enumerable.Empty<CourseDisplayDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses by category {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<IEnumerable<CourseDisplayDto>> GetCoursesByInstructorAsync(string instructorId)
        {
            try
            {
                var courses = await _httpClient.GetFromJsonAsync<IEnumerable<CourseDisplayDto>>($"api/courses/instructor/{instructorId}");
                return courses ?? Enumerable.Empty<CourseDisplayDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses by instructor {InstructorId}", instructorId);
                throw;
            }
        }

        // Note: Write operations would require authentication and proper API endpoints
        // These methods are stubs for now
        public async Task<Course> CreateCourseAsync(CourseFormDto courseDto, string instructorId)
        {
            throw new NotImplementedException("Course creation through API not implemented yet");
        }

        public async Task<Course> UpdateCourseAsync(int id, CourseFormDto courseDto)
        {
            throw new NotImplementedException("Course update through API not implemented yet");
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            throw new NotImplementedException("Course deletion through API not implemented yet");
        }

        public async Task<bool> PublishCourseAsync(int id)
        {
            throw new NotImplementedException("Course publishing through API not implemented yet");
        }

        public async Task<bool> UnpublishCourseAsync(int id)
        {
            throw new NotImplementedException("Course unpublishing through API not implemented yet");
        }

        #endregion

        #region Search and Filtering

        public async Task<IEnumerable<CourseDisplayDto>> SearchCoursesAsync(string searchTerm)
        {
            try
            {
                var courses = await _httpClient.GetFromJsonAsync<IEnumerable<CourseDisplayDto>>($"api/courses/search?term={Uri.EscapeDataString(searchTerm)}");
                return courses ?? Enumerable.Empty<CourseDisplayDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching courses with term {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<CourseDisplayDto>> GetCoursesByLevelAsync(CourseLevel level)
        {
            try
            {
                var courses = await _httpClient.GetFromJsonAsync<IEnumerable<CourseDisplayDto>>($"api/courses/level/{level}");
                return courses ?? Enumerable.Empty<CourseDisplayDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses by level {Level}", level);
                throw;
            }
        }

        #endregion

        #region Statistics

        public async Task<int> GetTotalCoursesCountAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<int>("api/courses/count");
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
                return await _httpClient.GetFromJsonAsync<int>("api/courses/published/count");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published courses count");
                throw;
            }
        }

        #endregion
    }
}