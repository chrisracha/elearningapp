using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELearningApp.Models.Entities
{
    public class CourseTag
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; } = null!;

        public int TagId { get; set; }

        [ForeignKey("TagId")]
        public Tag Tag { get; set; } = null!;
    }

    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<CourseTag> CourseTags { get; set; } = new List<CourseTag>();
    }
} 