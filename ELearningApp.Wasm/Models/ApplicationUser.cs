namespace ELearningApp.Wasm.Models
{
    // Simplified ApplicationUser for WebAssembly - no Identity dependencies
    public class ApplicationUser
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsInstructor { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Social Media Links
        public string? LinkedIn { get; set; }
        public string? Twitter { get; set; }
        public string? GitHub { get; set; }
        public string? Website { get; set; }
        
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}