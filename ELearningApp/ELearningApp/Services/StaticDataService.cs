using ELearningApp.Models.Entities;
using ELearningApp.Models.Enums;
using ELearningApp.Models.DTOs;

namespace ELearningApp.Services;

public class StaticDataService
{
    public List<Category> GetCategories()
    {
        return new List<Category>
        {
            new Category
            {
                Id = 1,
                Name = "Web Development",
                Description = "Learn modern web development technologies",
                IconUrl = "code",
                Color = "bg-blue-50",
                CourseCount = 127
            },
            new Category
            {
                Id = 2,
                Name = "Business Management",
                Description = "Learn key business skills for professional management",
                IconUrl = "briefcase",
                Color = "bg-emerald-50",
                CourseCount = 89
            },
            new Category
            {
                Id = 3,
                Name = "Graphic Design",
                Description = "Create stunning designs to communicate effectively",
                IconUrl = "paint-brush",
                Color = "bg-purple-50",
                CourseCount = 156
            },
            new Category
            {
                Id = 4,
                Name = "Programming",
                Description = "Master programming languages and frameworks",
                IconUrl = "terminal",
                Color = "bg-indigo-50",
                CourseCount = 203
            },
            new Category
            {
                Id = 5,
                Name = "Language Learning",
                Description = "Learn new languages with native speaker instructors",
                IconUrl = "language",
                Color = "bg-amber-50",
                CourseCount = 78
            },
            new Category
            {
                Id = 6,
                Name = "Data Science",
                Description = "Master data analysis and machine learning",
                IconUrl = "chart-bar",
                Color = "bg-indigo-50",
                CourseCount = 94
            },
            new Category
            {
                Id = 7,
                Name = "Data Analytics",
                Description = "Turn data into actionable business insights",
                IconUrl = "chart-pie",
                Color = "bg-rose-50",
                CourseCount = 112
            }
        };
    }

    public List<CourseDisplayDto> GetTopCourses()
    {
        return new List<CourseDisplayDto>
        {
            new CourseDisplayDto
            {
                Id = 1,
                Title = "Grounding Skills for PTSD, Stress, Panic and Anxiety",
                Description = "Learn to Turn off the Fight, Flight, Freeze Response and Turn on Calm Using your Body's Natural Parasympathetic Response",
                InstructorName = "INSTRUCTOR",
                ImageUrl = "https://images.unsplash.com/photo-1551288049-bebda4e38f71?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                YouTubeVideoId = "dQw4w9WgXcQ", // Sample YouTube video ID
                YouTubePreviewVideoId = "jNQXAC9IVRw", // Sample preview video ID
                Rating = 4.3,
                ReviewCount = 125840,
                Level = "Beginner",
                IsBestseller = true,
                Duration = "42 hours"
            },
            new CourseDisplayDto
            {
                Id = 2,
                Title = "Modern Web Development",
                Description = "Master modern web development, including HTML, CSS, and JavaScript, with practical projects and examples.",
                InstructorName = "Mark Smith",
                ImageUrl = "https://images.unsplash.com/photo-1593720213428-28a5b9e94613?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                YouTubeVideoId = "jNQXAC9IVRw", // Sample YouTube video ID
                YouTubePreviewVideoId = "dQw4w9WgXcQ", // Sample preview video ID
                Rating = 4.8,
                ReviewCount = 231565,
                Level = "Intermediate",
                IsBestseller = false,
                Duration = "56 hours"
            },
            new CourseDisplayDto
            {
                Id = 3,
                Title = "Complete Programming Course",
                Description = "Learn multiple programming languages including Python programming, including variables, loops, and functions, with hands-on exercises.",
                InstructorName = "John Doe",
                ImageUrl = "https://images.unsplash.com/photo-1517180102446-f3ece451e9d8?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                YouTubeVideoId = "dQw4w9WgXcQ", // Sample YouTube video ID
                YouTubePreviewVideoId = "jNQXAC9IVRw", // Sample preview video ID
                Rating = 4.7,
                ReviewCount = 187945,
                Level = "Beginner",
                IsBestseller = false,
                Duration = "38 hours"
            }
        };
    }

    public List<Company> GetTrustedCompanies()
    {
        return new List<Company>
        {
            new Company { Id = 1, Name = "Citi", LogoUrl = "https://logo.clearbit.com/citigroup.com" },
            new Company { Id = 2, Name = "P&G", LogoUrl = "https://logo.clearbit.com/pg.com" },
            new Company { Id = 3, Name = "Volkswagen", LogoUrl = "https://logo.clearbit.com/volkswagen.com" },
            new Company { Id = 4, Name = "PayPal", LogoUrl = "https://logo.clearbit.com/paypal.com" },
            new Company { Id = 5, Name = "Adobe", LogoUrl = "https://logo.clearbit.com/adobe.com" },
            new Company { Id = 6, Name = "Apple", LogoUrl = "https://logo.clearbit.com/apple.com" },
            new Company { Id = 7, Name = "Samsung", LogoUrl = "https://logo.clearbit.com/samsung.com" }
        };
    }

    public List<PricingPlan> GetPricingPlans()
    {
        return new List<PricingPlan>
        {
            new PricingPlan
            {
                Id = 1,
                Name = "Individual Plan",
                Description = "For individuals looking to learn",
                Price = 0,
                PriceDescription = "Free",
                IsPopular = false,
                ButtonText = "Get started",
                ButtonAction = "/register",
                Features = new List<string>
                {
                    "Access to all courses",
                    "Certificate of completion",
                    "Progress tracking",
                    "Community access"
                }
            },
            new PricingPlan
            {
                Id = 2,
                Name = "Student Plan",
                Description = "Perfect for students",
                Price = 0,
                PriceDescription = "Free",
                IsPopular = true,
                ButtonText = "Get started",
                ButtonAction = "/register",
                Features = new List<string>
                {
                    "Access to all courses",
                    "Certificate of completion",
                    "Progress tracking",
                    "Advanced learning tools",
                    "Priority support"
                }
            },
            new PricingPlan
            {
                Id = 3,
                Name = "Educator Plan",
                Description = "For educators and instructors",
                Price = 0,
                PriceDescription = "Free",
                IsPopular = false,
                ButtonText = "Get started",
                ButtonAction = "/register",
                Features = new List<string>
                {
                    "Access to all courses",
                    "Create and publish courses",
                    "Student analytics",
                    "Instructor tools",
                    "Community management"
                }
            }
        };
    }

    public List<Testimonial> GetTestimonials()
    {
        return new List<Testimonial>
        {
            new Testimonial
            {
                Id = 1,
                Name = "Chris T.",
                Content = "This platform transformed my career prospects. The comprehensive courses and practical projects gave me the confidence I needed for my next job interview.",
                AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?ixlib=rb-4.0.3&auto=format&fit=crop&w=150&q=80",
                Rating = 4.8,
                JobTitle = "Software Engineer"
            },
            new Testimonial
            {
                Id = 2,
                Name = "Emma S.",
                Content = "I love the variety of courses available. The instructors are amazing and the content is always up-to-date with industry standards.",
                AvatarUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?ixlib=rb-4.0.3&auto=format&fit=crop&w=150&q=80",
                Rating = 4.9,
                JobTitle = "UX Designer"
            },
            new Testimonial
            {
                Id = 3,
                Name = "David L.",
                Content = "The community and support here are amazing. I feel confident learning at my own pace and the instructors are always available to help.",
                AvatarUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-4.0.3&auto=format&fit=crop&w=150&q=80",
                Rating = 4.7,
                JobTitle = "Data Analyst"
            }
        };
    }
}