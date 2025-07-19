namespace ELearningApp.Models.Enums
{
    public enum CourseLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3,
        Expert = 4
    }

    public enum CourseStatus
    {
        Draft = 1,
        Published = 2,
        Archived = 3,
        Suspended = 4
    }

    public enum LessonType
    {
        Video = 1,
        Article = 2,
        ExternalLink = 3
    }

    public enum EnrollmentStatus
    {
        Active = 1,
        Completed = 2,
        Dropped = 3,
        Suspended = 4
    }

    public enum UserRole
    {
        Student = 1,
        Instructor = 2,
        Admin = 3
    }
} 