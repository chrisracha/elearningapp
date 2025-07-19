using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ELearningApp.Data;
using ELearningApp.Models.Enums;

namespace ELearningApp.Models.Entities
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [ForeignKey("StudentId")]
        public ApplicationUser Student { get; set; } = null!;

        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; } = null!;

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;

        public double ProgressPercentage { get; set; } = 0.0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; }

        // Navigation properties
        public ICollection<LessonProgress> LessonProgress { get; set; } = new List<LessonProgress>();
    }
} 