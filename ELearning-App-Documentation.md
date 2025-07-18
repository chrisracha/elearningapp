# ELearning App - Project Documentation

## 📋 Project Overview

The ELearning App is a comprehensive online learning platform built with **Blazor WebAssembly** and **.NET 9**. It provides a modern, interactive learning experience with both server-side and client-side rendering capabilities, designed to deliver scalable educational content to users worldwide.

## 🛠 Technology Stack

- **Framework**: .NET 9
- **Frontend**: Blazor WebAssembly + Blazor Server (Hybrid)
- **Authentication**: ASP.NET Core Identity
- **Database**: SQL Server with Entity Framework Core
- **UI Framework**: Bootstrap 5
- **Architecture**: Clean Architecture with separation of concerns
- **Rendering**: Interactive Server and WebAssembly components

## 📁 Workspace Information

### Directory Structure
```
C:\Users\SAM\source\repos\ELearningApp\
├── ELearningApp\                    # Main server application
│   ├── Components\                  # Blazor components
│   ├── Data\                       # Database context and models
│   ├── wwwroot\                    # Static assets
│   └── ELearningApp.csproj        # Main project file
└── ELearningApp.Client\            # WebAssembly client
    ├── Pages\                      # Client-side pages
    ├── wwwroot\                    # Client assets
    └── ELearningApp.Client.csproj  # Client project file
```

### Projects in Solution

| Project Name | Framework | Type | Purpose |
|---|---|---|---|
| ELearningApp | .NET 9 | Blazor Server | Main application with server-side rendering |
| ELearningApp.Client | .NET 9 | Blazor WebAssembly | Client-side application for enhanced interactivity |

## 🚀 Key Features

### 1. 🔐 Authentication & User Management
- **User Registration & Login** - Complete ASP.NET Core Identity integration
- **Password Recovery** - Forgot password functionality with email confirmation
- **Two-Factor Authentication** - Enhanced security with authenticator apps
- **External Login Providers** - Support for third-party authentication
- **User Profile Management** - Account settings and personal data management
- **Email Confirmation** - Secure account verification process

### 2. 📚 Course Management
- **Create Course** - Instructors can create comprehensive courses
- **Edit Course** - Modify existing course content and structure
- **Delete Course** - Remove courses from the platform
- **Upload Content** - Support for various multimedia content types
- **Course Publishing** - Control course visibility and availability

### 3. 🎓 Course Viewing & Learning Experience
- **Enrolled Course Viewing** - Seamless access to enrolled courses
- **Unenrolled Course Viewing** - Browse and preview available courses
- **Interactive Course Content** - Rich multimedia learning materials
- **Progress Tracking** - Real-time learning progress monitoring
- **Video Streaming** - Integrated video content delivery system
- **Responsive Design** - Optimized for desktop and mobile devices

#### Content Types Supported:
- 📹 **Video Lessons** - High-quality video streaming
- 📄 **Articles** - Rich text content with formatting
- 🔗 **External Links** - Integration with external resources
- 📢 **Announcements** - Course updates and notifications
- 📊 **Reviews** - Student feedback and ratings system

### 4. 🔍 Course Discovery
- **Course Catalog** - Comprehensive course browsing interface
- **Search Functionality** - Find courses by keywords and filters
- **Advanced Filtering** - Refine course listings by various criteria
- **Detailed Course Information** - Comprehensive course descriptions
- **Student Reviews** - Peer feedback and rating system
- **Course Recommendations** - Personalized course suggestions

### 5. 📊 Learning Dashboard
- **My Learning Dashboard** - Personalized learning hub
- **Resume Learning** - Continue from last session
- **Enrolled Courses Overview** - Quick access to active courses
- **Progress Visualization** - Visual progress indicators and statistics
- **Learning Analytics** - Track learning patterns and achievements

## 🗄 Data Models

### Core Entities

#### ApplicationUser
```csharp
public class ApplicationUser : IdentityUser
{
    // Extends ASP.NET Core Identity User
    // Properties: Id, UserName, Email, PasswordHash, PhoneNumber, etc.
    // Additional properties can be added for profile information
}
```

#### Course (Planned Implementation)
```csharp
public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string InstructorId { get; set; }
    public ApplicationUser Instructor { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsPublished { get; set; }
    public decimal Price { get; set; }
    public string ThumbnailUrl { get; set; }
    public string Category { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public DifficultyLevel Level { get; set; }
    
    // Navigation Properties
    public ICollection<Module> Modules { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<Review> Reviews { get; set; }
}
```

#### Module (Planned Implementation)
```csharp
public class Module
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public int SortOrder { get; set; }
    public bool IsPreview { get; set; }
    
    // Navigation Properties
    public ICollection<Lesson> Lessons { get; set; }
}
```

#### Lesson (Planned Implementation)
```csharp
public class Lesson
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public LessonType Type { get; set; } // Video, Article, Quiz, Assignment
    public int ModuleId { get; set; }
    public Module Module { get; set; }
    public int SortOrder { get; set; }
    public TimeSpan Duration { get; set; }
    public string VideoUrl { get; set; }
    public string ArticleContent { get; set; }
    public string ExternalLink { get; set; }
    public bool IsRequired { get; set; }
    
    // Navigation Properties
    public ICollection<UserProgress> UserProgresses { get; set; }
}
```

#### Enrollment (Planned Implementation)
```csharp
public class Enrollment
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal ProgressPercentage { get; set; }
    public bool IsCertificateIssued { get; set; }
    public EnrollmentStatus Status { get; set; }
}
```

#### UserProgress (Planned Implementation)
```csharp
public class UserProgress
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public DateTime LastAccessedDate { get; set; }
    public int AttemptsCount { get; set; }
}
```

#### Review (Planned Implementation)
```csharp
public class Review
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string Title { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsRecommended { get; set; }
}
```

### Enumerations
```csharp
public enum LessonType
{
    Video,
    Article,
    Quiz,
    Assignment,
    ExternalLink,
    Wireframe
}

public enum DifficultyLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}

public enum EnrollmentStatus
{
    Active,
    Completed,
    Suspended,
    Dropped
}
```

## 🏗 Component Architecture

### Feature-Based Organization
```
Components/
├── Account/                    # Authentication & user management
│   ├── Pages/                 # Account-related pages
│   │   ├── Login.razor
│   │   ├── Register.razor
│   │   └── Manage/           # Account management
│   └── Shared/               # Shared account components
├── Layout/                    # Application layouts
│   ├── MainLayout.razor
│   └── NavMenu.razor
└── Pages/                     # Main application pages
    ├── Home.razor
    ├── Weather.razor           # Sample component
    └── Counter.razor           # Sample component
```

### Feature Modules (Planned)
```
Features/
├── Authentication/
│   ├── Components/
│   │   ├── Login.razor
│   │   ├── Register.razor
│   │   └── ForgotPassword.razor
├── CourseManagement/
│   ├── Components/
│   │   ├── CreateCourse.razor
│   │   ├── EditCourse.razor
│   │   ├── DeleteCourse.razor
│   │   └── UploadContent.razor
├── CourseViewing/
│   ├── EnrolledCourseViewing/
│   │   ├── ViewCourseContent.razor
│   │   ├── ViewAnnouncementTab.razor
│   │   ├── ViewArticle.razor
│   │   ├── ViewReviewsTab.razor
│   │   ├── ViewVideo.razor
│   └── UnenrolledCourseViewing/
│       ├── CourseContentTab.razor
│       ├── ReviewsTab.razor
│       └── ViewInstructor/
├── CourseCatalog/
│   ├── Components/
│   │   ├── BrowseCourses.razor
│   │   ├── FilterCourses.razor
│   │   ├── SearchCourses.razor
│   │   └── ViewCourseDetails.razor
└── MyLearningDashboard/
    ├── Components/
    │   ├── ResumeLearning.razor
    │   ├── ViewEnrolledCourses.razor
    │   └── ViewProgress.razor
```

## 📦 Key Dependencies

### Server Project (ELearningApp.csproj)
```xml
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Server" Version="9.0.6" />
<PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="9.0.6" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="9.0.6" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.6" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.6" />
```

### Client Project (ELearningApp.Client.csproj)
```xml
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.6" />
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Authentication" Version="9.0.6" />
```

## 🔒 Security Features

- **ASP.NET Core Identity** - Comprehensive user management
- **Role-based Authorization** - Different access levels (Student, Instructor, Admin)
- **Antiforgery Protection** - CSRF attack prevention
- **Secure Password Policies** - Configurable password requirements
- **Two-Factor Authentication** - TOTP support with authenticator apps
- **External Authentication** - OAuth integration capabilities
- **Data Protection** - Personal data download and deletion compliance
- **Secure Endpoints** - Protected API endpoints with proper authorization

## 🚀 Getting Started

### Prerequisites
- **.NET 9 SDK** - Latest version
- **SQL Server** - LocalDB or full SQL Server instance
- **Visual Studio 2022** or **VS Code** with C# extension
- **Node.js** (optional, for additional tooling)

### Setup Instructions
1. **Clone Repository**
   ```bash
   git clone [repository-url]
   cd ELearningApp
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Database Setup**
   ```bash
   dotnet ef database update
   ```

4. **Run Application**
   ```bash
   dotnet run --project ELearningApp
   ```

### Configuration
- Update connection string in `appsettings.json`
- Configure email settings for account confirmation
- Set up external authentication providers (Google, Microsoft, etc.)
- Configure logging and monitoring

## 📈 Development Status

| Feature | Status | Description |
|---------|--------|-------------|
| ✅ Authentication System | **Complete** | Full Identity implementation with 2FA |
| ✅ Project Architecture | **Complete** | Blazor hybrid setup established |
| ✅ Component Structure | **Complete** | Feature-based organization |
| 🚧 Course Management | **In Progress** | UI components and data models |
| 🚧 Learning Interface | **Planned** | Student learning experience |
| 🚧 Video Integration | **Planned** | Streaming and progress tracking |
| 🚧 Search & Discovery | **Planned** | Course catalog functionality |
| 🚧 Analytics Dashboard | **Planned** | Learning analytics and reporting |

### Core Learning Platform
- **Complete Course Management** - Full CRUD operations
- **Video Streaming Integration** - Azure Media Services or similar
- **Progress Tracking** - Real-time learning analytics
- **Basic Messaging** - Instructor-student communication

## 🏆 Architecture Benefits

### Blazor Hybrid Approach
- **Rich Client Experience** - Interactive WebAssembly components
- **Fast Initial Load** - Server-side rendering for SEO
- **Code Sharing** - Shared components between server and client
- **Type Safety** - Full C# stack with compile-time checking
- **Modern Development** - Latest .NET features and tooling

### Scalability Features
- **Component Reusability** - Modular architecture
- **Clean Separation** - Feature-based organization
- **Database Flexibility** - Entity Framework abstraction
- **Cloud Ready** - Designed for Azure deployment
- **Performance Optimized** - Streaming rendering and lazy loading

## 📞 Support & Documentation

- **Technical Documentation** - Comprehensive code documentation
- **API Documentation** - RESTful API endpoints
- **Component Library** - Reusable Blazor components
- **Development Guidelines** - Coding standards and best practices
- **Deployment Guide** - Production deployment instructions

---

*This documentation reflects the current state of the ELearning App project and will be updated as development progresses.*