using Microsoft.AspNetCore.Identity;
using ELearningApp.Models.Entities;

namespace ELearningApp.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsInstructor { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Profile Information (only fields that exist in database)
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        
        // Additional fields commented out until proper migration is created
        // public string? CompanyName { get; set; }
        // public string? WebsiteUrl { get; set; }
        // public string? LinkedInUrl { get; set; }
        // public string? TwitterUrl { get; set; }
        
        // Navigation properties
        public virtual ICollection<Course> CoursesAsInstructor { get; set; } = new List<Course>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();
    }
}
