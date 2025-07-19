using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using ELearningApp.Data;
using System.Security.Claims;

namespace ELearningApp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<UserService> _logger;

        public UserService(
            ApplicationDbContext context,
            AuthenticationStateProvider authStateProvider,
            ILogger<UserService> logger)
        {
            _context = context;
            _authStateProvider = authStateProvider;
            _logger = logger;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            try
            {
                return await _context.Users.FindAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID {UserId}", userId);
                throw;
            }
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            try
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return null;

                return await GetUserByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                throw;
            }
        }

        public async Task<bool> IsInstructorAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                return user?.IsInstructor == true && user.IsActive;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking instructor status for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> MakeUserInstructorAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return false;

                user.IsInstructor = true;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} has been granted instructor role", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making user {UserId} an instructor", userId);
                throw;
            }
        }

        public async Task<bool> RemoveInstructorRoleAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return false;

                user.IsInstructor = false;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Instructor role removed from user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing instructor role from user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(ApplicationUser user)
        {
            try
            {
                user.UpdatedAt = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated profile for user {UserId}", user.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for {UserId}", user.Id);
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetInstructorsAsync()
        {
            try
            {
                return await _context.Users
                    .Where(u => u.IsInstructor && u.IsActive)
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting instructors list");
                throw;
            }
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return false;

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Deactivated user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ActivateUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return false;

                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Activated user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", userId);
                throw;
            }
        }
    }
} 