using ELearningApp.Data;

namespace ELearningApp.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetCurrentUserAsync();
        Task<bool> IsInstructorAsync(string userId);
        Task<bool> MakeUserInstructorAsync(string userId);
        Task<bool> RemoveInstructorRoleAsync(string userId);
        Task<bool> UpdateUserProfileAsync(ApplicationUser user);
        Task<IEnumerable<ApplicationUser>> GetInstructorsAsync();
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ActivateUserAsync(string userId);
    }
} 