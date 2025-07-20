using ELearningApp.Wasm.Models.Entities;

namespace ELearningApp.Wasm.Services
{
    // Simplified interface for WebAssembly HTTP client
    public interface ISimpleCourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<IEnumerable<Course>> GetFeaturedCoursesAsync();
    }
}