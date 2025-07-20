using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ELearningApp.Api.Data;
using ELearningApp.Api.Models.Enums;

namespace ELearningApp.Api.Models.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string ShortDescription { get; set; } = string.Empty;

        [StringLength(2000)]
        public string LongDescription { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        // YouTube Video Integration
        public string? YouTubeVideoId { get; set; }  // For preview videos
        public string? YouTubePreviewVideoId { get; set; }  // For course previews
        
        public string? WhatYouWillLearn { get; set; }
        public string? Prerequisites { get; set; }
        public CourseLevel Level { get; set; } = CourseLevel.Beginner;
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public bool IsPublished { get; set; } = false;
        public int EstimatedDurationMinutes { get; set; }
        
        // Category
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        
        // Instructor
        [ForeignKey("Instructor")]
        public string InstructorId { get; set; } = string.Empty;
        public virtual ApplicationUser Instructor { get; set; } = null!;
        
        // Statistics
        public int EnrollmentCount { get; set; } = 0;
        public int ViewCount { get; set; } = 0;
        public double AverageRating { get; set; } = 0.0;
        public int ReviewCount { get; set; } = 0;
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
        public virtual ICollection<CourseTag> CourseTags { get; set; } = new List<CourseTag>();
    }
} 