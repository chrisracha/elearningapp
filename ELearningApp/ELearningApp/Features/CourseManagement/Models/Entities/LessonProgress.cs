using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ELearningApp.Data;

namespace ELearningApp.Models.Entities
{
    public class LessonProgress
    {
        [Key]
        public int Id { get; set; }

        public int EnrollmentId { get; set; }

        [ForeignKey("EnrollmentId")]
        public Enrollment Enrollment { get; set; } = null!;

        public int LessonId { get; set; }

        [ForeignKey("LessonId")]
        public Lesson Lesson { get; set; } = null!;

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;

        public int TimeSpentMinutes { get; set; } = 0;

        public double ProgressPercentage { get; set; } = 0.0;
    }
} 