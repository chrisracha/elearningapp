using Microsoft.AspNetCore.Identity;

namespace ELearningApp.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsInstructor { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
