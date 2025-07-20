using System.Net.Http.Json;
using ELearningApp.Wasm.Models.Entities;

namespace ELearningApp.Wasm.Services
{
    public class SimpleCourseService : ISimpleCourseService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SimpleCourseService> _logger;

        public SimpleCourseService(HttpClient httpClient, ILogger<SimpleCourseService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            try
            {
                var courses = await _httpClient.GetFromJsonAsync<IEnumerable<Course>>("api/courses");
                return courses ?? Enumerable.Empty<Course>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all courses");
                return Enumerable.Empty<Course>();
            }
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Course>($"api/courses/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course by ID {CourseId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<Course>> GetFeaturedCoursesAsync()
        {
            try
            {
                var courses = await _httpClient.GetFromJsonAsync<IEnumerable<Course>>("api/courses/featured");
                return courses ?? Enumerable.Empty<Course>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured courses");
                return Enumerable.Empty<Course>();
            }
        }
    }
}