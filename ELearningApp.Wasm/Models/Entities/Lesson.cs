using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ELearningApp.Wasm.Models.Enums;

namespace ELearningApp.Wasm.Models.Entities
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(5000)]
        public string? Content { get; set; }

        [StringLength(500)]
        public string? VideoUrl { get; set; }

        [StringLength(500)]
        public string? ArticleUrl { get; set; }

        [StringLength(500)]
        public string? DownloadUrl { get; set; }

        public LessonType Type { get; set; } = LessonType.Article;

        public int OrderIndex { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsFree { get; set; } = false;

        public int ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public Module Module { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<LessonProgress> LessonProgress { get; set; } = new List<LessonProgress>();
    }
} 