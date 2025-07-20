namespace ELearningApp.Models.DTOs
{
    public class CourseDisplayDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string InstructorId { get; set; } = string.Empty;
        public string? InstructorProfileImageUrl { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? YouTubeVideoId { get; set; }
        public string? YouTubePreviewVideoId { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int CategoryId { get; set; }
        public string Level { get; set; } = string.Empty;
        public bool IsBestseller { get; set; }
        public string Duration { get; set; } = string.Empty;
    }

    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
    }

    public class PricingPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Period { get; set; } = string.Empty;
        public string PriceDescription { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new List<string>();
        public bool IsPopular { get; set; }
        public string ButtonText { get; set; } = string.Empty;
        public string ButtonClass { get; set; } = string.Empty;
        public string ButtonAction { get; set; } = string.Empty;
    }

    public class Testimonial
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public double Rating { get; set; }
    }
}