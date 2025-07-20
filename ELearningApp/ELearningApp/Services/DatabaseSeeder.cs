using ELearningApp.Data;
using ELearningApp.Models.Entities;
using ELearningApp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ELearningApp.Services
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();
            
            // Check if we already have users
            if (await _userManager.Users.AnyAsync())
            {
                // Update existing instructor images if they're using old URLs
                await UpdateInstructorImagesAsync();
                return; // Database already seeded
            }

            // Seed categories
            await SeedCategoriesAsync();
            
            // Seed users (instructor with courses, instructor without courses, student)
            await SeedUsersAsync();
            
            // Seed comprehensive Blazor course
            await SeedBlazorCourseAsync();
            
            // Seed other courses
            await SeedOtherCoursesAsync();
            
            // Seed student enrollments
            await SeedEnrollmentsAsync();
        }

        private async Task UpdateInstructorImagesAsync()
        {
            var sarahJohnson = await _userManager.FindByEmailAsync("blazor.instructor@example.com");
            var michaelChen = await _userManager.FindByEmailAsync("new.instructor@example.com");
            var emilyDavis = await _userManager.FindByEmailAsync("student@example.com");

            bool updated = false;

            if (sarahJohnson != null && (string.IsNullOrEmpty(sarahJohnson.ProfileImageUrl) || sarahJohnson.ProfileImageUrl.Contains("randomuser.me")))
            {
                sarahJohnson.ProfileImageUrl = "https://media-s3-us-east-1.ceros.com/forbes/images/2024/07/30/5b8cedef33c0398f1baf3553ea7a3d9d/1x1.jpg?imageOpt=1&fit=bounds&width=1082";
                await _userManager.UpdateAsync(sarahJohnson);
                updated = true;
            }

            if (michaelChen != null && (string.IsNullOrEmpty(michaelChen.ProfileImageUrl) || michaelChen.ProfileImageUrl.Contains("randomuser.me")))
            {
                michaelChen.ProfileImageUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80";
                await _userManager.UpdateAsync(michaelChen);
                updated = true;
            }

            if (emilyDavis != null && (string.IsNullOrEmpty(emilyDavis.ProfileImageUrl) || emilyDavis.ProfileImageUrl.Contains("randomuser.me")))
            {
                emilyDavis.ProfileImageUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80";
                await _userManager.UpdateAsync(emilyDavis);
                updated = true;
            }

            if (updated)
            {
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedCategoriesAsync()
        {
            var categories = new[]
            {
                new Category { Name = "Web Development", Description = "Learn modern web development technologies", IconUrl = "code", Color = "bg-blue-50", CourseCount = 127 },
                new Category { Name = "Business Management", Description = "Learn key business skills for professional management", IconUrl = "briefcase", Color = "bg-emerald-50", CourseCount = 89 },
                new Category { Name = "Graphic Design", Description = "Create stunning designs to communicate effectively", IconUrl = "paint-brush", Color = "bg-purple-50", CourseCount = 156 },
                new Category { Name = "Programming", Description = "Master programming languages and frameworks", IconUrl = "terminal", Color = "bg-indigo-50", CourseCount = 203 }
            };

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
        }

        private async Task SeedUsersAsync()
        {
            // Create instructor with courses
            var instructorWithCourses = new ApplicationUser
            {
                UserName = "blazor.instructor@example.com",
                Email = "blazor.instructor@example.com",
                EmailConfirmed = true,
                FirstName = "Sarah",
                LastName = "Johnson",
                IsInstructor = true,
                IsActive = true,
                Bio = "Senior Software Engineer with 8+ years of experience in .NET and Blazor development. Passionate about teaching modern web development.",
                ProfileImageUrl = "https://media-s3-us-east-1.ceros.com/forbes/images/2024/07/30/5b8cedef33c0398f1baf3553ea7a3d9d/1x1.jpg?imageOpt=1&fit=bounds&width=1082",
                CompanyName = "TechCorp Solutions",
                WebsiteUrl = "https://sarahjohnson.dev",
                LinkedInUrl = "https://linkedin.com/in/sarahjohnson",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create instructor without courses
            var instructorWithoutCourses = new ApplicationUser
            {
                UserName = "new.instructor@example.com",
                Email = "new.instructor@example.com",
                EmailConfirmed = true,
                FirstName = "Michael",
                LastName = "Chen",
                IsInstructor = true,
                IsActive = true,
                Bio = "Experienced developer transitioning into teaching. Specializing in cloud computing and DevOps.",
                ProfileImageUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                CompanyName = "CloudTech Inc",
                WebsiteUrl = "https://michaelchen.dev",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create student
            var student = new ApplicationUser
            {
                UserName = "student@example.com",
                Email = "student@example.com",
                EmailConfirmed = true,
                FirstName = "Emily",
                LastName = "Davis",
                IsInstructor = false,
                IsActive = true,
                Bio = "Aspiring web developer learning modern technologies. Currently focused on .NET and Blazor.",
                ProfileImageUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create users
            await _userManager.CreateAsync(instructorWithCourses, "Password123!");
            await _userManager.CreateAsync(instructorWithoutCourses, "Password123!");
            await _userManager.CreateAsync(student, "Password123!");
        }

        private async Task SeedBlazorCourseAsync()
        {
            var blazorInstructor = await _userManager.FindByEmailAsync("blazor.instructor@example.com");
            var programmingCategory = await _context.Categories.FirstAsync(c => c.Name == "Programming");

            // Create comprehensive Blazor course
            var blazorCourse = new Course
            {
                Title = "Complete Blazor Development: From Beginner to Expert",
                ShortDescription = "Master Blazor Server and WebAssembly with real-world projects, best practices, and advanced techniques",
                LongDescription = @"This comprehensive course takes you from the basics of Blazor to advanced enterprise-level development. You'll learn both Blazor Server and Blazor WebAssembly, understand the differences, and know when to use each approach.

What you'll learn:
• Blazor Server fundamentals and architecture
• Blazor WebAssembly setup and deployment
• Component development and lifecycle
• State management and data binding
• Authentication and authorization
• Real-time communication with SignalR
• Integration with Entity Framework Core
• Advanced patterns and best practices
• Performance optimization techniques
• Deployment strategies for production

This course includes hands-on projects, real-world examples, and practical exercises to reinforce your learning.",
                ImageUrl = "https://images.unsplash.com/photo-1517077304055-6e89abbf09b0?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                YouTubeVideoId = "QnBtqkKDuGo", // Blazor tutorial video
                YouTubePreviewVideoId = "Ku6Wa0omTZA", // Blazor setup tutorial
                WhatYouWillLearn = @"• Build modern web applications with Blazor
• Understand the difference between Server and WebAssembly
• Implement authentication and authorization
• Create reusable components and layouts
• Work with Entity Framework Core
• Deploy applications to production
• Optimize performance and user experience
• Follow best practices and patterns",
                Prerequisites = @"• Basic knowledge of C# programming
• Understanding of HTML, CSS, and JavaScript
• Familiarity with web development concepts
• Visual Studio or VS Code installed",
                Level = CourseLevel.Intermediate,
                Status = CourseStatus.Published,
                IsPublished = true,
                EstimatedDurationMinutes = 3600, // 60 hours
                CategoryId = programmingCategory.Id,
                InstructorId = blazorInstructor!.Id,
                AverageRating = 4.9,
                ReviewCount = 2847,
                EnrollmentCount = 15678,
                ViewCount = 45678,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            };

            _context.Courses.Add(blazorCourse);
            await _context.SaveChangesAsync();

            // Create modules for the Blazor course
            var modules = new[]
            {
                new Module
                {
                    Title = "Getting Started with Blazor",
                    Description = "Introduction to Blazor, setup, and basic concepts",
                    OrderIndex = 1,
                    CourseId = blazorCourse.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Module
                {
                    Title = "Blazor Server Fundamentals",
                    Description = "Deep dive into Blazor Server architecture and components",
                    OrderIndex = 2,
                    CourseId = blazorCourse.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Module
                {
                    Title = "Blazor WebAssembly",
                    Description = "Building client-side applications with Blazor WebAssembly",
                    OrderIndex = 3,
                    CourseId = blazorCourse.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Module
                {
                    Title = "Component Development",
                    Description = "Creating reusable components and understanding lifecycle",
                    OrderIndex = 4,
                    CourseId = blazorCourse.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Module
                {
                    Title = "Data Management",
                    Description = "Working with data, Entity Framework, and APIs",
                    OrderIndex = 5,
                    CourseId = blazorCourse.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Module
                {
                    Title = "Authentication & Authorization",
                    Description = "Implementing security in Blazor applications",
                    OrderIndex = 6,
                    CourseId = blazorCourse.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Module
                {
                    Title = "Real-time Communication",
                    Description = "Using SignalR for real-time features",
                    OrderIndex = 7,
                    CourseId = blazorCourse.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Module
                {
                    Title = "Advanced Patterns",
                    Description = "Advanced patterns, performance optimization, and best practices",
                    OrderIndex = 8,
                    CourseId = blazorCourse.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _context.Modules.AddRange(modules);
            await _context.SaveChangesAsync();

            // Create lessons for each module
            var lessons = new List<Lesson>();

            // Module 1: Getting Started
            var module1 = modules[0];
            lessons.AddRange(new[]
            {
                new Lesson
                {
                    Title = "What is Blazor?",
                    Description = "Introduction to Blazor and its benefits",
                    Content = @"Blazor is a framework for building interactive web UIs using C# instead of JavaScript. It allows you to write client-side web applications using C# and .NET.

Key benefits:
• Write C# instead of JavaScript
• Share code between client and server
• Leverage existing .NET ecosystem
• Strong typing and IntelliSense
• Component-based architecture",
                    Type = LessonType.Article,
                    OrderIndex = 1,
                    DurationMinutes = 15,
                    IsFree = true,
                    ModuleId = module1.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Lesson
                {
                    Title = "Setting Up Your Development Environment",
                    Description = "Install and configure tools for Blazor development",
                    VideoUrl = "https://youtu.be/yBw_KOz1vCA?si=nGcNwQFv54K-fvWF",
                    Type = LessonType.Video,
                    OrderIndex = 2,
                    DurationMinutes = 25,
                    IsFree = true,
                    ModuleId = module1.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Lesson
                {
                    Title = "Your First Blazor Application",
                    Description = "Create and run your first Blazor app",
                    Content = @"Let's create your first Blazor application step by step.

1. Open Visual Studio
2. Create a new Blazor Server App
3. Explore the project structure
4. Run the application
5. Understand the basic components

The default Blazor Server template includes:
• A main layout with navigation
• Sample pages (Home, Counter, Weather)
• Basic styling with Bootstrap
• Authentication setup (optional)",
                    Type = LessonType.Article,
                    OrderIndex = 3,
                    DurationMinutes = 30,
                    IsFree = false,
                    ModuleId = module1.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });

            // Module 2: Blazor Server Fundamentals
            var module2 = modules[1];
            lessons.AddRange(new[]
            {
                new Lesson
                {
                    Title = "Blazor Server Architecture",
                    Description = "Understanding how Blazor Server works",
                    Content = @"Blazor Server runs on the server and uses SignalR to communicate with the client. The UI updates are sent to the browser as HTML diffs.

Architecture components:
• SignalR connection for real-time communication
• Server-side rendering of components
• State management on the server
• Automatic reconnection handling
• Scalability considerations",
                    Type = LessonType.Article,
                    OrderIndex = 1,
                    DurationMinutes = 20,
                    IsFree = false,
                    ModuleId = module2.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Lesson
                {
                    Title = "Component Lifecycle",
                    Description = "Understanding component lifecycle methods",
                    VideoUrl = "https://youtu.be/_m3egdHi8Bo?si=yTQ9-GSETycYvR9w",
                    Type = LessonType.Video,
                    OrderIndex = 2,
                    DurationMinutes = 35,
                    IsFree = false,
                    ModuleId = module2.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Lesson
                {
                    Title = "Data Binding and Events",
                    Description = "Working with data binding and event handling",
                    Content = @"Blazor provides powerful data binding capabilities that make it easy to keep your UI in sync with your data.

Key concepts:
• One-way binding (@variable)
• Two-way binding (@bind-value)
• Event handling (@onclick, @onchange)
• Parameter passing
• Cascading parameters",
                    Type = LessonType.Article,
                    OrderIndex = 3,
                    DurationMinutes = 25,
                    IsFree = false,
                    ModuleId = module2.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });

            // Module 3: Blazor WebAssembly
            var module3 = modules[2];
            lessons.AddRange(new[]
            {
                new Lesson
                {
                    Title = "Blazor WebAssembly vs Server",
                    Description = "Comparing the two Blazor hosting models",
                    Content = @"Understanding when to use Blazor Server vs Blazor WebAssembly is crucial for building the right application.

Blazor Server:
• Runs on server, UI updates via SignalR
• Faster initial load, smaller client bundle
• Requires constant connection
• Better for intranet applications

Blazor WebAssembly:
• Runs entirely in browser
• Works offline after initial load
• Larger initial download
• Better for public web applications",
                    Type = LessonType.Article,
                    OrderIndex = 1,
                    DurationMinutes = 20,
                    IsFree = false,
                    ModuleId = module3.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Lesson
                {
                    Title = "Setting Up Blazor WebAssembly",
                    Description = "Create and configure a Blazor WebAssembly project",
                    VideoUrl = "https://youtu.be/GHDgR_-poc4?si=N9xm4vNSrcD88sxM",
                    Type = LessonType.Video,
                    OrderIndex = 2,
                    DurationMinutes = 30,
                    IsFree = false,
                    ModuleId = module3.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });

            // Add more lessons for remaining modules...
            // Module 4: Component Development
            var module4 = modules[3];
            lessons.AddRange(new[]
            {
                new Lesson
                {
                    Title = "Creating Reusable Components",
                    Description = "Build components that can be shared across your application",
                    Content = @"Components are the building blocks of Blazor applications. Learn how to create reusable, maintainable components.

Best practices:
• Single responsibility principle
• Parameter design
• Event callbacks
• Component composition
• Naming conventions",
                    Type = LessonType.Article,
                    OrderIndex = 1,
                    DurationMinutes = 25,
                    IsFree = false,
                    ModuleId = module4.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });

            // Module 5: Data Management
            var module5 = modules[4];
            lessons.AddRange(new[]
            {
                new Lesson
                {
                    Title = "Working with Entity Framework Core",
                    Description = "Integrate Entity Framework Core with Blazor",
                    Content = @"Entity Framework Core provides a powerful way to work with databases in your Blazor applications.

Topics covered:
• Setting up DbContext
• Creating models and migrations
• CRUD operations
• Data validation
• Performance optimization",
                    Type = LessonType.Article,
                    OrderIndex = 1,
                    DurationMinutes = 30,
                    IsFree = false,
                    ModuleId = module5.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Lesson
                {
                    Title = "API Integration",
                    Description = "Consuming REST APIs in Blazor applications",
                    VideoUrl = "https://youtu.be/0bOP3HheD2k?si=cqYjtmxr1jZNbFxn",
                    Type = LessonType.Video,
                    OrderIndex = 2,
                    DurationMinutes = 35,
                    IsFree = false,
                    ModuleId = module5.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });

            // Module 6: Authentication & Authorization
            var module6 = modules[5];
            lessons.AddRange(new[]
            {
                new Lesson
                {
                    Title = "Identity and Authentication",
                    Description = "Implement user authentication in Blazor",
                    Content = @"Secure your Blazor applications with proper authentication and authorization.

Implementation steps:
• Configure Identity services
• Create login/register pages
• Implement authorization policies
• Handle user roles and claims
• Secure API endpoints",
                    Type = LessonType.Article,
                    OrderIndex = 1,
                    DurationMinutes = 40,
                    IsFree = false,
                    ModuleId = module6.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });

            // Module 7: Real-time Communication
            var module7 = modules[6];
            lessons.AddRange(new[]
            {
                new Lesson
                {
                    Title = "SignalR Integration",
                    Description = "Add real-time features to your Blazor applications",
                    VideoUrl = "https://youtu.be/RBVIclt4sOo?si=smBSo4ogGJf-NaoO",
                    Type = LessonType.Video,
                    OrderIndex = 1,
                    DurationMinutes = 45,
                    IsFree = false,
                    ModuleId = module7.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });

            // Module 8: Advanced Patterns
            var module8 = modules[7];
            lessons.AddRange(new[]
            {
                new Lesson
                {
                    Title = "Performance Optimization",
                    Description = "Optimize your Blazor applications for better performance",
                    Content = @"Learn advanced techniques to improve the performance of your Blazor applications.

Optimization strategies:
• Component virtualization
• Lazy loading
• Memory management
• Bundle optimization
• Caching strategies",
                    Type = LessonType.Article,
                    OrderIndex = 1,
                    DurationMinutes = 35,
                    IsFree = false,
                    ModuleId = module8.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Lesson
                {
                    Title = "Deployment Strategies",
                    Description = "Deploy your Blazor applications to production",
                    VideoUrl = "https://youtu.be/vgCmkL4l0h8?si=7o-U8Jb97YQC3VQJ",
                    Type = LessonType.Video,
                    OrderIndex = 2,
                    DurationMinutes = 30,
                    IsFree = false,
                    ModuleId = module8.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });

            _context.Lessons.AddRange(lessons);
            await _context.SaveChangesAsync();
        }

        private async Task SeedOtherCoursesAsync()
        {
            var blazorInstructor = await _userManager.FindByEmailAsync("blazor.instructor@example.com");
            var webDevCategory = await _context.Categories.FirstAsync(c => c.Name == "Web Development");
            var businessCategory = await _context.Categories.FirstAsync(c => c.Name == "Business Management");

            var otherCourses = new[]
            {
                new Course
                {
                    Title = "Modern Web Development with React",
                    ShortDescription = "Learn React.js from scratch with modern practices and real-world projects",
                    LongDescription = "A comprehensive React course covering hooks, context, routing, and state management.",
                    ImageUrl = "https://images.unsplash.com/photo-1633356122544-f134324a6cee?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                    YouTubeVideoId = "LvBBnMFNGbo", // React tutorial video
                    YouTubePreviewVideoId = "SqcY0GlETPk", // React basics video
                    Level = CourseLevel.Intermediate,
                    Status = CourseStatus.Published,
                    IsPublished = true,
                    EstimatedDurationMinutes = 2400, // 40 hours
                    CategoryId = webDevCategory.Id,
                    InstructorId = blazorInstructor!.Id,
                    AverageRating = 4.7,
                    ReviewCount = 15432,
                    EnrollmentCount = 98765,
                    ViewCount = 234567,
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    UpdatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Title = "Business Strategy Fundamentals",
                    ShortDescription = "Master the fundamentals of business strategy and competitive advantage",
                    LongDescription = "Learn how to develop and implement effective business strategies.",
                    ImageUrl = "https://images.unsplash.com/photo-1552664730-d307ca884978?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                    YouTubeVideoId = "3ex8VWulUks", // Business strategy video
                    YouTubePreviewVideoId = "8YJS85UzMEE", // Business fundamentals video
                    Level = CourseLevel.Beginner,
                    Status = CourseStatus.Published,
                    IsPublished = true,
                    EstimatedDurationMinutes = 1800, // 30 hours
                    CategoryId = businessCategory.Id,
                    InstructorId = blazorInstructor.Id,
                    AverageRating = 4.5,
                    ReviewCount = 8765,
                    EnrollmentCount = 54321,
                    ViewCount = 123456,
                    CreatedAt = DateTime.UtcNow.AddDays(-90),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _context.Courses.AddRange(otherCourses);
            await _context.SaveChangesAsync();
        }

        private async Task SeedEnrollmentsAsync()
        {
            var student = await _userManager.FindByEmailAsync("student@example.com");
            var allCourses = await _context.Courses.ToListAsync();

            var enrollments = allCourses.Select(course => new Enrollment
            {
                UserId = student!.Id,
                CourseId = course.Id,
                EnrollmentDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = Random.Shared.Next(0, 100),
                LastAccessedDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 7))
            }).ToList();

            _context.Enrollments.AddRange(enrollments);
            await _context.SaveChangesAsync();

            // Create some lesson progress records
            var lessonProgresses = new List<LessonProgress>();
            var allLessons = await _context.Lessons.ToListAsync();

            foreach (var enrollment in enrollments)
            {
                var courseLessons = allLessons.Where(l => l.Module.CourseId == enrollment.CourseId).ToList();
                var completedLessons = courseLessons.Take(Random.Shared.Next(0, courseLessons.Count)).ToList();

                foreach (var lesson in completedLessons)
                {
                    lessonProgresses.Add(new LessonProgress
                    {
                        EnrollmentId = enrollment.Id,
                        LessonId = lesson.Id,
                        IsCompleted = true,
                        CompletedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)),
                        TimeSpentMinutes = Random.Shared.Next(10, lesson.DurationMinutes),
                        LastAccessedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 7)),
                        ProgressPercentage = 100.0
                    });
                }
            }

            _context.LessonProgress.AddRange(lessonProgresses);
            await _context.SaveChangesAsync();

            // Create sample announcements
            await SeedAnnouncementsAsync();
        }

        private async Task SeedAnnouncementsAsync()
        {
            var blazorInstructor = await _userManager.FindByEmailAsync("blazor.instructor@example.com");
            var allCourses = await _context.Courses.ToListAsync();

            var announcements = new List<CourseAnnouncement>();

            foreach (var course in allCourses)
            {
                // Add 2-3 announcements per course
                announcements.AddRange(new[]
                {
                    new CourseAnnouncement
                    {
                        CourseId = course.Id,
                        InstructorId = blazorInstructor!.Id,
                        Title = "Welcome to the Course!",
                        Content = $@"Dear Students,

Welcome to {course.Title}! I'm excited to have you join this learning journey.

Here are a few important things to get started:
• Make sure to follow the lessons in order for the best learning experience
• Don't hesitate to ask questions in the discussion section
• Take your time with each module - understanding is more important than speed
• Practice the exercises and projects to reinforce your learning

Looking forward to seeing your progress!

Best regards,
Sarah Johnson",
                        IsImportant = false,
                        IsPublished = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(15, 30)),
                        UpdatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(15, 30))
                    },
                    new CourseAnnouncement
                    {
                        CourseId = course.Id,
                        InstructorId = blazorInstructor.Id,
                        Title = "Important: Updated Course Materials",
                        Content = $@"Hello Everyone,

I've just updated some of the course materials with the latest information and best practices.

What's New:
• Updated code examples in Module 3
• New resource links added to Module 5
• Fixed typos in several lesson descriptions
• Added bonus content for advanced learners

Please make sure to check out the updated content. If you've already completed those modules, it's worth taking a quick look at the new materials.

Thanks for your attention!",
                        IsImportant = true,
                        IsPublished = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(5, 15)),
                        UpdatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(5, 15))
                    },
                    new CourseAnnouncement
                    {
                        CourseId = course.Id,
                        InstructorId = blazorInstructor.Id,
                        Title = "Tips for Success",
                        Content = @"Hi Students,

Based on feedback from previous cohorts, here are some tips to help you succeed in this course:

1. **Set a Schedule**: Try to dedicate consistent time each week to the course
2. **Practice Coding**: Don't just watch - code along with the examples
3. **Join the Community**: Connect with other students for support and collaboration
4. **Ask Questions**: No question is too basic - everyone is here to learn
5. **Build Projects**: Apply what you learn by building your own projects

Remember, learning to code is a journey, not a sprint. Be patient with yourself and celebrate small victories along the way.

You've got this! 💪",
                        IsImportant = false,
                        IsPublished = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 7)),
                        UpdatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 7))
                    }
                });
            }

            _context.CourseAnnouncements.AddRange(announcements);
            await _context.SaveChangesAsync();
        }
    }
}