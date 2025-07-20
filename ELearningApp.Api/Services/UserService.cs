using ELearningApp.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ELearningApp.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _logger = logger;
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return await _userManager.GetUserAsync(user);
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
} 