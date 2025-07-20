using ELearningApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ELearningApp.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                // Use a separate context to avoid concurrency issues
                using var scope = _serviceProvider.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await userManager.GetUserAsync(user);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return null;
        }
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        try
        {
            return await _userManager.FindByIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> IsUserInstructorAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.IsInstructor == true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is instructor", userId);
            return false;
        }
    }

    public async Task<bool> SetUserInstructorStatusAsync(string userId, bool isInstructor)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.IsInstructor = isInstructor;
            var result = await _userManager.UpdateAsync(user);

            _logger.LogInformation("Updated instructor status for user {UserId} to {IsInstructor}", userId, isInstructor);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting instructor status for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> UpdateUserAsync(ApplicationUser user)
    {
        try
        {
            user.UpdatedAt = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("Updated user profile for user {UserId}", user.Id);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to update user {UserId}: {Errors}", user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", user.Id);
            return false;
        }
    }
} 