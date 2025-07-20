using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ELearningApp.Wasm.Data;

namespace ELearningApp.Wasm.Models.Entities
{
    public class CourseReview
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; } = null!;

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [ForeignKey("StudentId")]
        public ApplicationUser Student { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? ReviewText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsVerifiedPurchase { get; set; } = false;
    }
} 