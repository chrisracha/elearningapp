using ELearningApp.Wasm.Models;

namespace ELearningApp.Wasm.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetCurrentUserAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<bool> UpdateUserProfileAsync(ApplicationUser user);
    Task<bool> IsUserInstructorAsync(string userId);
} 