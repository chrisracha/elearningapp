# Database Seeder Documentation

This document describes the comprehensive database seeder that populates the e-learning application with sample data for development and testing purposes.

## Overview

The `DatabaseSeeder` service automatically creates sample data when the application runs in development mode. It ensures that the database has realistic data for testing all features of the e-learning platform.

## What Gets Seeded

### 1. Categories
- **Web Development** - Learn modern web development technologies
- **Business Management** - Learn key business skills for professional management  
- **Graphic Design** - Create stunning designs to communicate effectively
- **Programming** - Master programming languages and frameworks

### 2. Users

#### Instructor with Courses
- **Email**: `blazor.instructor@example.com`
- **Password**: `Password123!`
- **Name**: Sarah Johnson
- **Bio**: Senior Software Engineer with 8+ years of experience in .NET and Blazor development
- **Courses**: Creates the comprehensive Blazor course and other sample courses

#### Instructor without Courses
- **Email**: `new.instructor@example.com`
- **Password**: `Password123!`
- **Name**: Michael Chen
- **Bio**: Experienced developer transitioning into teaching. Specializing in cloud computing and DevOps
- **Courses**: None (as requested)

#### Student (Enrolled in All Courses)
- **Email**: `student@example.com`
- **Password**: `Password123!`
- **Name**: Emily Davis
- **Bio**: Aspiring web developer learning modern technologies
- **Enrollments**: Automatically enrolled in all available courses

### 3. Comprehensive Blazor Course

**Title**: "Complete Blazor Development: From Beginner to Expert"

**Course Details**:
- **Duration**: 60 hours (3600 minutes)
- **Level**: Intermediate
- **Category**: Programming
- **Rating**: 4.9/5 (2,847 reviews)
- **Enrollments**: 15,678 students
- **Views**: 45,678

**Course Content**:
- 8 comprehensive modules
- Mix of video lessons and articles
- Real YouTube video integration
- Progressive learning path from basics to advanced topics

**Modules**:
1. **Getting Started with Blazor** - Introduction and setup
2. **Blazor Server Fundamentals** - Architecture and components
3. **Blazor WebAssembly** - Client-side development
4. **Component Development** - Reusable components
5. **Data Management** - Entity Framework and APIs
6. **Authentication & Authorization** - Security implementation
7. **Real-time Communication** - SignalR integration
8. **Advanced Patterns** - Performance and deployment

**Lesson Types**:
- **Video Lessons**: YouTube video integration with real tutorial videos
- **Article Lessons**: Comprehensive written content with code examples
- **Mixed Content**: Both video and article content for comprehensive learning

### 4. Additional Courses

#### Modern Web Development with React
- **Duration**: 40 hours
- **Level**: Intermediate
- **Category**: Web Development
- **Rating**: 4.7/5

#### Business Strategy Fundamentals
- **Duration**: 30 hours
- **Level**: Beginner
- **Category**: Business Management
- **Rating**: 4.5/5

### 5. Student Enrollments

The student (`student@example.com`) is automatically enrolled in all courses with:
- Random enrollment dates (within the last 30 days)
- Random progress percentages (0-100%)
- Random last access dates
- Lesson progress tracking for completed lessons

## How to Use

### Automatic Seeding
The seeder runs automatically when the application starts in development mode. It only seeds if no users exist in the database.

### Manual Seeding
You can also trigger the seeder manually by injecting the `DatabaseSeeder` service and calling `SeedAsync()`.

### Resetting the Database
To reset and reseed the database:
1. Delete the existing database
2. Restart the application
3. The seeder will automatically create fresh sample data

## Sample Login Credentials

### For Testing as Instructor with Courses
- **Email**: `blazor.instructor@example.com`
- **Password**: `Password123!`

### For Testing as Instructor without Courses
- **Email**: `new.instructor@example.com`
- **Password**: `Password123!`

### For Testing as Student
- **Email**: `student@example.com`
- **Password**: `Password123!`

## Features Demonstrated

The seeded data demonstrates:
- ✅ Comprehensive Blazor course with YouTube videos and articles
- ✅ Student enrolled to every course
- ✅ Instructor with no courses
- ✅ Realistic course statistics and ratings
- ✅ Multiple lesson types (video, article)
- ✅ Progress tracking and enrollment data
- ✅ Category organization
- ✅ User profiles with bios and social links

## Technical Implementation

The seeder is implemented as a service (`DatabaseSeeder`) that:
- Uses dependency injection for database context and user manager
- Creates realistic, varied sample data
- Handles relationships between entities properly
- Only seeds if the database is empty
- Uses proper async/await patterns
- Includes error handling and validation

## Customization

To customize the seeded data:
1. Modify the `DatabaseSeeder.cs` file
2. Add new seed methods for additional data types
3. Update the `SeedAsync()` method to call your new methods
4. Restart the application to see changes

The seeder is designed to be easily extensible for adding more sample data as the application grows. 