using ELearningApp.Api.Data;

namespace ELearningApp.Api.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetCurrentUserAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<bool> IsUserInstructorAsync(string userId);
    Task<bool> SetUserInstructorStatusAsync(string userId, bool isInstructor);
} 