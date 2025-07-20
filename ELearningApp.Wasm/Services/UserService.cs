using System.Net.Http.Json;
using ELearningApp.Wasm.Models;

namespace ELearningApp.Wasm.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;

        public UserService(HttpClient httpClient, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ApplicationUser>("api/user/current");
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
                return await _httpClient.GetFromJsonAsync<ApplicationUser>($"api/user/{userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(ApplicationUser user)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/user/profile", user);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return false;
            }
        }

        // Simplified method signatures for WASM - no need for complex server operations
        public async Task<bool> IsUserInstructorAsync(string userId)
        {
            var user = await GetUserByIdAsync(userId);
            return user?.IsInstructor ?? false;
        }
    }
}