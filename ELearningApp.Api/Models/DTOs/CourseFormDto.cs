using System.ComponentModel.DataAnnotations;
using ELearningApp.Api.Models.Enums;

namespace ELearningApp.Api.Models.DTOs
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

        [StringLength(50, ErrorMessage = "YouTube Video ID cannot exceed 50 characters")]
        public string? YouTubeVideoId { get; set; }

        [StringLength(50, ErrorMessage = "YouTube Preview Video ID cannot exceed 50 characters")]
        public string? YouTubePreviewVideoId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Course level is required")]
        public CourseLevel Level { get; set; } = CourseLevel.Beginner;

        [StringLength(1000, ErrorMessage = "Prerequisites cannot exceed 1000 characters")]
        public string? Prerequisites { get; set; }

        [StringLength(1000, ErrorMessage = "What you will learn cannot exceed 1000 characters")]
        public string? WhatYouWillLearn { get; set; }

        [Range(1, 10000, ErrorMessage = "Duration must be between 1 and 10,000 minutes")]
        public int EstimatedDurationMinutes { get; set; }
    }

    public class CourseUpdateDto : CourseCreateDto
    {
        [Required]
        public int Id { get; set; }
    }

    public class CourseFormDto : CourseCreateDto
    {
        public int? Id { get; set; }
        public string? InstructorId { get; set; }
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
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
        public double? MinRating { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public string SortBy { get; set; } = "newest"; // newest, rating, popular
    }

    public class CourseStatsDto
    {
        public int TotalCourses { get; set; }
        public int PublishedCourses { get; set; }
        public int DraftCourses { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
    }
} 