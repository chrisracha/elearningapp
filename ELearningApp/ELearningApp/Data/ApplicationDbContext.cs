using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ELearningApp.Models.Entities;

namespace ELearningApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // Course Management DbSets
        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<LessonProgress> LessonProgress { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<CourseAnnouncement> CourseAnnouncements { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CourseTag> CourseTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Course relationships properly
            builder.Entity<Course>(entity =>
            {
                // Configure the instructor relationship
                entity.HasOne(c => c.Instructor)
                      .WithMany()
                      .HasForeignKey(c => c.InstructorId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure the category relationship
                entity.HasOne(c => c.Category)
                      .WithMany(cat => cat.Courses)
                      .HasForeignKey(c => c.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Module relationships
            builder.Entity<Module>()
                .HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Lesson relationships
            builder.Entity<Lesson>()
                .HasOne(l => l.Module)
                .WithMany(m => m.Lessons)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Enrollment relationships
            builder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure LessonProgress relationships
            builder.Entity<LessonProgress>()
                .HasOne(lp => lp.Enrollment)
                .WithMany(e => e.LessonProgresses)
                .HasForeignKey(lp => lp.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LessonProgress>()
                .HasOne(lp => lp.Lesson)
                .WithMany(l => l.LessonProgress)
                .HasForeignKey(lp => lp.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure CourseReview relationships
            builder.Entity<CourseReview>()
                .HasOne(cr => cr.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(cr => cr.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseReview>()
                .HasOne(cr => cr.Student)
                .WithMany()
                .HasForeignKey(cr => cr.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure CourseAnnouncement relationships
            builder.Entity<CourseAnnouncement>()
                .HasOne(ca => ca.Course)
                .WithMany()
                .HasForeignKey(ca => ca.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseAnnouncement>()
                .HasOne(ca => ca.Instructor)
                .WithMany()
                .HasForeignKey(ca => ca.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure CourseTag relationships (many-to-many)
            builder.Entity<CourseTag>()
                .HasOne(ct => ct.Course)
                .WithMany(c => c.CourseTags)
                .HasForeignKey(ct => ct.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseTag>()
                .HasOne(ct => ct.Tag)
                .WithMany(t => t.CourseTags)
                .HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure unique constraints
            builder.Entity<Enrollment>()
                .HasIndex(e => new { e.UserId, e.CourseId })
                .IsUnique();

            builder.Entity<LessonProgress>()
                .HasIndex(lp => new { lp.EnrollmentId, lp.LessonId })
                .IsUnique();

            builder.Entity<CourseReview>()
                .HasIndex(cr => new { cr.StudentId, cr.CourseId })
                .IsUnique();

            builder.Entity<CourseTag>()
                .HasIndex(ct => new { ct.CourseId, ct.TagId })
                .IsUnique();

            // Configure decimal precision
            // Pricing configurations removed - all courses are free
        }
    }
}
