using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ELearningApp.Data;
using ELearningApp.Models.Enums;

namespace ELearningApp.Models.Entities
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
        public string? LongDescription { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [StringLength(500)]
        public string? VideoPreviewUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }

        public CourseLevel Level { get; set; } = CourseLevel.Beginner;

        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        [Required]
        public string InstructorId { get; set; } = string.Empty;

        [ForeignKey("InstructorId")]
        public ApplicationUser Instructor { get; set; } = null!;

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        [StringLength(1000)]
        public string? Prerequisites { get; set; }

        [StringLength(1000)]
        public string? WhatYouWillLearn { get; set; }

        public int EstimatedDurationMinutes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PublishedAt { get; set; }

        public bool IsFeatured { get; set; } = false;

        public int ViewCount { get; set; } = 0;

        public int EnrollmentCount { get; set; } = 0;

        public double AverageRating { get; set; } = 0.0;

        public int ReviewCount { get; set; } = 0;

        // Navigation properties
        public ICollection<Module> Modules { get; set; } = new List<Module>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
        public ICollection<CourseTag> CourseTags { get; set; } = new List<CourseTag>();
    }
} 