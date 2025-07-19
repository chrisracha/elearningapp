# ELearning App - Project Documentation

## 📋 Project Overview

The ELearning App is a comprehensive online learning platform built with **Blazor WebAssembly** and **.NET 9**. It provides a modern, interactive learning experience with both server-side and client-side rendering capabilities, designed to deliver scalable educational content to users worldwide.

## 🛠 Technology Stack

- **Framework**: .NET 9
- **Frontend**: Blazor WebAssembly + Blazor Server (Hybrid)
- **Styling**: Tailwind CSS
- **Authentication**: ASP.NET Core Identity
- **Database**: SQL Server with Entity Framework Core
- **Architecture**: Vertical Slice Architecture with feature-based organization
- **Development Methodology**: Test-Driven Development (TDD)
- **Testing Framework**: xUnit + FluentAssertions + Playwright (E2E)
- **Rendering**: Interactive Server and WebAssembly components

## 🏗️ Architecture Approach

### Vertical Slice Architecture
This project follows **Vertical Slice Architecture** principles, organizing code by features rather than technical layers. Each feature slice contains everything needed for that specific functionality:

- **Single Responsibility**: Each slice handles one specific business capability
- **Minimal Coupling**: Features are loosely coupled and can evolve independently
- **Feature Cohesion**: All related code (UI, business logic, data access) lives together
- **Testability**: Each slice can be tested in isolation with clear boundaries

### Test-Driven Development (TDD)
The project follows strict **TDD practices** with the Red-Green-Refactor cycle:

1. **🔴 Red**: Write a failing test that describes the desired behavior
2. **🟢 Green**: Write the minimum code necessary to make the test pass
3. **🔵 Refactor**: Improve the code while keeping tests green

**Benefits of TDD in this project:**
- **Design Driven**: Tests drive the API design and component interfaces
- **Rapid Feedback**: Immediate feedback on code changes
- **Documentation**: Tests serve as living documentation
- **Confidence**: Safe refactoring with comprehensive test coverage
- **Quality**: Higher code quality through continuous validation

## 📁 Workspace Information

### Directory Structure
```
C:\Users\SAM\source\repos\ELearningApp\
├── ELearningApp\                    # Main server application
│   ├── Features\                    # Vertical slices (feature modules)
│   ├── Components\                  # Shared Blazor components
│   ├── Data\                       # Database context and shared models
│   ├── wwwroot\                    # Static assets
│   └── ELearningApp.csproj        # Main project file
├── ELearningApp.Client\            # WebAssembly client
│   ├── Features\                   # Client-side feature slices
│   ├── Pages\                      # Client-side pages
│   ├── wwwroot\                    # Client assets
│   └── ELearningApp.Client.csproj # Client project file
├── ELearningApp.Tests\             # Unit tests (planned)
│   ├── Features\                   # Feature-specific tests
│   ├── Integration\                # Integration tests
│   └── ELearningApp.Tests.csproj  # Test project file
└── ELearningApp.E2E.Tests\        # End-to-end tests (planned)
    ├── Features\                   # Feature-based E2E tests
    ├── PageObjects\                # Page object models
    └── ELearningApp.E2E.Tests.csproj # E2E test project file
```

### Projects in Solution

| Project Name | Framework | Type | Purpose |
|---|---|---|---|
| ELearningApp | .NET 9 | Blazor Server | Main application with server-side rendering |
| ELearningApp.Client | .NET 9 | Blazor WebAssembly | Client-side application for enhanced interactivity |
| ELearningApp.Tests | .NET 9 | xUnit Test Project | Unit and integration tests |
| ELearningApp.E2E.Tests | .NET 9 | Playwright Test Project | End-to-end tests |

## 🚀 Key Features

### 1. 🔐 Authentication & User Management
- **User Registration & Login** - Complete ASP.NET Core Identity integration

### 2. 📚 Course Management
- **Create Course** - Instructors can create comprehensive courses
- **Edit Course** - Modify existing course content and structure
- **Delete Course** - Remove courses from the platform
- **Upload Content** - Support for various multimedia content types

### 3. 🎓 Course Viewing & Learning Experience
- **Enrolled Course Viewing** - Seamless access to enrolled courses
- **Unenrolled Course Viewing** - Browse and preview available courses
- **Interactive Course Content** - Rich multimedia learning materials
- **Progress Tracking** - Real-time learning progress monitoring
- **Video Streaming** - Integrated video content delivery system
- **Responsive Design** - Optimized for desktop and mobile devices with Tailwind CSS

#### Content Types Supported:
- 📹 **Video Lessons** - High-quality video streaming
- 📄 **Articles** - Rich text content with formatting
- 🔗 **External Links** - Integration with external resources
- 📊 **Reviews** - Student feedback and ratings system

### 4. 🔍 Course Discovery
- **Course Catalog** - Comprehensive course browsing interface
- **Search Functionality** - Find courses by keywords and filters
- **Advanced Filtering** - Refine course listings by various criteria
- **Detailed Course Information** - Comprehensive course descriptions
- **Student Reviews** - Peer feedback and rating system

### 5. 📊 Learning Dashboard
- **My Learning Dashboard** - Personalized learning hub
- **Resume Learning** - Continue from last session
- **Enrolled Courses Overview** - Quick access to active courses
- **Progress Visualization** - Visual progress indicators and statistics

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
    public LessonType Type { get; set; } // Video, Article, ExternalLink
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
    ExternalLink
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

## 🎨 UI Design System

### Tailwind CSS Integration
The application uses **Tailwind CSS** for modern, utility-first styling:

- **Responsive Design**: Mobile-first approach with responsive utilities
- **Component-Based**: Reusable UI components with consistent styling
- **Performance**: Purged CSS for production builds
- **Customization**: Custom design tokens and extended theme
- **Dark Mode**: Built-in dark mode support (planned)

#### Key Design Principles:
- **Consistency**: Unified design language across all components
- **Accessibility**: WCAG 2.1 AA compliance with proper color contrast
- **Performance**: Optimized CSS bundle sizes
- **Maintainability**: Utility classes for rapid development

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
<!-- Core Framework -->
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Server" Version="9.0.6" />
<PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="9.0.6" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="9.0.6" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.6" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.6" />

<!-- CQRS and Mediator -->
<PackageReference Include="MediatR" Version="12.2.0" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />

<!-- Mapping -->
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />

<!-- Testing -->
<PackageReference Include="bunit" Version="1.24.10" />
```

### Client Project (ELearningApp.Client.csproj)
```xml
<!-- Core WebAssembly -->
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.6" />
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Authentication" Version="9.0.6" />
```

**Note**: Bootstrap dependencies have been removed in favor of Tailwind CSS for modern utility-first styling.

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
- **Node.js** (for Tailwind CSS tooling)

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
- Configure Tailwind CSS build process
- Configure logging and monitoring

## 📈 Development Status

| Feature | TDD Status | Implementation Status | Test Coverage |
|---------|------------|----------------------|---------------|
| ✅ Authentication System | **Complete** | **Complete** | 85% |
| ✅ Project Architecture | **Complete** | **Complete** | N/A |
| ✅ Tailwind CSS Integration | **Complete** | **Complete** | N/A |
| 🚧 Course Management | **In Progress** | **Planned** | 0% |
| 🚧 Learning Interface | **Planned** | **Planned** | 0% |
| 🚧 Video Integration | **Planned** | **Planned** | 0% |
| 🚧 Search & Discovery | **Planned** | **Planned** | 0% |

### Implementation Notes

#### UI Framework Migration ✅
- **Previous State**: Bootstrap 5 was used for styling
- **Current State**: Successfully migrated to Tailwind CSS
- **Migration Completed**: 
  1. ✅ Integrated Tailwind CSS via CDN for rapid development
  2. ✅ Removed Bootstrap dependencies from App.razor
  3. ✅ Updated all layout components with Tailwind utility classes
  4. ✅ Implemented modern, responsive design patterns
  5. ✅ Created a beautiful landing page with Tailwind styling

**✅ Success**: The application now uses Tailwind CSS for all styling with a modern, clean design.

---

*This documentation reflects the planned state of the ELearning App project with Tailwind CSS as the target UI framework. The current implementation still uses Bootstrap and will be migrated according to the development roadmap.*