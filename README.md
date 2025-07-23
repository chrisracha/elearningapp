# ELearning App

<img width="1853" height="922" alt="image" src="https://github.com/user-attachments/assets/ac9844f0-5469-495c-8ec8-a0217eb3cd2d" />
<img width="1850" height="920" alt="image" src="https://github.com/user-attachments/assets/f141746d-e319-46cf-99e3-f128f1b3081f" />
<img width="1856" height="922" alt="image" src="https://github.com/user-attachments/assets/0157d760-f48d-464f-bcb4-dbb2bb0b34c4" />
<img width="1846" height="918" alt="image" src="https://github.com/user-attachments/assets/846edd85-f8f2-45fc-b377-b62fc06d3da4" />



## 📋 Project Overview

The ELearning App is a modern online learning platform built with **Blazor** (.NET 9), featuring both server-side and client-side rendering. It supports user registration, course creation and management, interactive course viewing, and a responsive, Tailwind CSS-powered UI.

---

## 🚀 Current Features

- **Authentication & User Management**
  - User registration, login, and role-based access (Student, Instructor)
  - ASP.NET Core Identity integration

- **Course Management**
  - Instructors can create, edit, and manage courses
  - Upload and organize course content (video, articles, links)
  - Category-based course organization

- **Course Viewing & Enrollment**
  - Students can browse, preview, and enroll in courses
  - Enrolled students can access full course content and track progress
  - Course recommendations and instructor profiles

- **Learning Dashboard**
  - Personalized dashboard for enrolled courses and progress tracking

- **UI/UX**
  - Fully migrated to Tailwind CSS for modern, responsive design
  - Accessible and mobile-friendly layouts

---

## 🏗️ Project Structure

- **ELearningApp/**: Main Blazor Server project (features, components, data, services)
- **ELearningApp.Client/**: Blazor WebAssembly client (for hybrid scenarios)
- **ELearningApp.Shared/**: Shared models and enums

---

## 🛠️ Technology Stack

- .NET 9, Blazor (Server & WASM)
- Tailwind CSS
- SQL Server + Entity Framework Core
- ASP.NET Core Identity

---

## 🗄️ Data Models

- **User**: Extends IdentityUser, supports instructor/student roles
- **Course**: Title, description, instructor, category, modules, lessons, reviews, etc.
- **Category**: Name, description, icon, color, etc.
- **Enrollment**: Tracks student progress in courses
- **Review**: Student feedback and ratings

---

## 📈 Development Status

| Feature                | Status         |
|------------------------|---------------|
| Authentication         | ✅ Complete   |
| Course Management      | ✅ Complete   |
| Course Viewing         | ✅ Complete   |
| Learning Dashboard     | ✅ Complete   |
| Tailwind CSS Migration | ✅ Complete   |
| Testing                | 🟡 Partial (unit tests planned) |

---

## 📝 Credentials (Demo)

- **Student:** `student@example.com` / `Password123!`
- **Instructor:** `blazor.instructor@example.com` / `Password123!`
- **Instructor (Empty):** `new.instructor@example.com` / `Password123!`
---

## 🚀 Getting Started

1. **Clone the repo**
2. **Restore dependencies:** `dotnet restore`
3. **Update the database:** `dotnet ef database update`
4. **Run the app:** `dotnet run --project ELearningApp`

---

## 🗒️ Notes

- Some advanced features (E2E tests, advanced analytics, dark mode) are planned but not yet implemented.
- The UI is fully migrated to Tailwind CSS.
- Course creation, management, enrollment, and dashboard are fully functional.
- Video streaming and review/rating are available.
