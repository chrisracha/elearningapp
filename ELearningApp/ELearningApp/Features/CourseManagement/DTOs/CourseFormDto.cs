using System.ComponentModel.DataAnnotations;
using ELearningApp.Models.Enums;

namespace ELearningApp.Models.DTOs
{
    public class CourseCreateDto
    {
        [Required(ErrorMessage = "Course title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Short description is required")]
        [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters")]
        public string ShortDescription { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Long description cannot exceed 2000 characters")]
        public string? LongDescription { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }

        [StringLength(500, ErrorMessage = "Video preview URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? VideoPreviewUrl { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 99999.99, ErrorMessage = "Price must be between 0 and 99,999.99")]
        public decimal Price { get; set; }

        [Range(0, 99999.99, ErrorMessage = "Discount price must be between 0 and 99,999.99")]
        public decimal? DiscountPrice { get; set; }

        [Required(ErrorMessage = "Course level is required")]
        public CourseLevel Level { get; set; } = CourseLevel.Beginner;

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        public int CategoryId { get; set; }

        [StringLength(1000, ErrorMessage = "Prerequisites cannot exceed 1000 characters")]
        public string? Prerequisites { get; set; }

        [StringLength(1000, ErrorMessage = "Learning outcomes cannot exceed 1000 characters")]
        public string? WhatYouWillLearn { get; set; }

        [Required(ErrorMessage = "Estimated duration is required")]
        [Range(1, 10000, ErrorMessage = "Duration must be between 1 and 10,000 minutes")]
        public int EstimatedDurationMinutes { get; set; }

        public bool IsFeatured { get; set; } = false;

        // Helper property for duration in hours and minutes
        public string EstimatedDurationDisplay => 
            $"{EstimatedDurationMinutes / 60}h {EstimatedDurationMinutes % 60}m";

        // Validation method
        public bool IsValid(out List<string> errors)
        {
            errors = new List<string>();

            if (DiscountPrice.HasValue && DiscountPrice >= Price)
            {
                errors.Add("Discount price must be less than the regular price");
            }

            if (EstimatedDurationMinutes <= 0)
            {
                errors.Add("Duration must be greater than 0");
            }

            return errors.Count == 0;
        }
    }

    public class CourseUpdateDto : CourseCreateDto
    {
        [Required]
        public int Id { get; set; }

        public CourseStatus Status { get; set; }

        public DateTime? PublishedAt { get; set; }

        public bool CanChangeStatus { get; set; } = true;
    }

    public class CourseStatusUpdateDto
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public CourseStatus Status { get; set; }
    }

    public class CourseSearchDto
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public CourseLevel? Level { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinRating { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public string SortBy { get; set; } = "newest"; // newest, rating, price, popular
    }

    public class CourseStatsDto
    {
        public int TotalCourses { get; set; }
        public int PublishedCourses { get; set; }
        public int DraftCourses { get; set; }
        public int TotalStudents { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageRating { get; set; }
    }
} 