using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ELearningApp.Wasm.Data;
using ELearningApp.Wasm.Models.Enums;

namespace ELearningApp.Wasm.Models.Entities;

public class Enrollment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    [Required]
    public int CourseId { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    [Required]
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public DateTime? CompletionDate { get; set; }

    public DateTime? LastAccessedDate { get; set; }

    [Range(0, 100)]
    public double ProgressPercentage { get; set; } = 0;

    public int TimeSpentMinutes { get; set; } = 0;

    public bool IsCertificateIssued { get; set; } = false;

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    // Navigation Properties
    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
} 