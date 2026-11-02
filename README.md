# University Management System

An ASP.NET Core MVC platform for managing a university: departments, instructors, students, courses, enrollments, attendance, exams, assignments, grades, fees, real-time chat and notifications, audit logs, and report generation.

> **Terminology note:** this project uses **Instructor** (not "Teacher") consistently across the codebase, database schema, and UI.

## Solution structure

| Project | Purpose |
|---|---|
| `UniversityManagementSystem.Domain` | Entities and enums (e.g., `Instructor`, `Student`, `Course`, `Exam`, `Assignment`) |
| `UniversityManagementSystem.Application` | Business logic — services, DTOs, AutoMapper profiles, validation |
| `UniversityManagementSystem.Infrastructure` | EF Core `ApplicationDbContext`, Identity configuration, repositories, migrations |
| `UniversityManagementSystem.Web` | ASP.NET Core MVC UI — controllers, Razor views, SignalR hubs, static assets |

## Tech stack

- **.NET 10** / ASP.NET Core MVC (Razor views)
- **Entity Framework Core 10** with **SQL Server**
- **ASP.NET Core Identity** — roles: `Admin`, `Instructor`, `Student`
- **SignalR** — real-time chat and notifications
- **AutoMapper** — DTO mapping
- **EPPlus** — Excel export · **iTextSharp (LGPL)** — PDF export
- **Bootstrap 5** + **Font Awesome** — UI

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (the default connection string points to the local `GEBRE\SQLEXPRESS` instance)

## Getting started

1. Clone / open the solution `UniversityManagementSystem.slnx` (Visual Studio 2022+ or any editor).
2. Update the connection string if needed in `UniversityManagementSystem.Web/appsettings.json` (and `appsettings.Development.json`):

   ```json
   "DefaultConnection": "Server=YOUR_SERVER;Database=UniversityManagementSystemDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
   ```

3. Restore and build:

   ```bash
   dotnet restore UniversityManagementSystem.slnx
   dotnet build UniversityManagementSystem.slnx
   ```

4. Run the application:

   ```bash
   dotnet run --project UniversityManagementSystem.Web
   ```

   On startup the app applies any pending migrations automatically. No seed data is inserted — only admins create users through the application.

   The app listens on **http://localhost:5134** (and `https://localhost:7177` for the `https` profile).

   To apply migrations manually instead:

   ```bash
   dotnet ef database update --project UniversityManagementSystem.Infrastructure --startup-project UniversityManagementSystem.Web
   ```

### Quick start (one command)

The repo includes launcher scripts that kill any leftover app instance first, then start the app on **http://localhost:5134**:

- **Windows:** double-click `kill-server.bat`
- **git-bash:** `./kill-server.sh`

> If port `5134` is already in use, the app automatically falls back to the next free port (e.g. `5135`) instead of failing to start — the chosen port is printed in the console.

### Troubleshooting: "address already in use"

If you see `Failed to bind to address ... address already in use`, a previous instance of the app is still running. Fix it by running:

```bash
taskkill //F //IM UniversityManagementSystem.Web.exe   # Windows
# or simply: kill-server.bat
```

Then start the app again.

## Default accounts

> Seed data has been removed — only admins create users through the application.

| Role | Username | Password |
|---|---|---|
| Admin | `admin` | `admin123!` |
| Instructor | *(set by admin)* | *(set by admin)* |
| Student | *(their Student UGR/StudentId)* | *(set by admin)* |

**Notes:**
- The admin creates students and instructors through the Admin panel.
- Student login username is their **Student UGR / StudentId** (e.g., `STU001`).
- Instructor login username is set by the admin when creating the instructor.
- All users log in with **username + password** (not email).

## Features by role

**Admin**
- Department, instructor, student, course, and enrollment management
- User management (create/edit/activate/deactivate users, assign roles)
- Attendance overview & reports, fee structures & payments, financial reports
- Audit logs, report generation & export (Excel / PDF), real-time chat

**Instructor**
- Course and student management for assigned courses
- Assignments (with questions) and exams (with questions), publishing and grading
- Manual essay grading, grade management
- Attendance marking and history

**Student**
- Enrollment & approvals, assignments and exams (take, submit, view results)
- Attendance, grades (with GPA), fees & receipts
- Profile and real-time chat with instructors/admin

**All users**
- Profile management (picture, contact info), change password, password reset, notifications via SignalR

## Database & migrations

- Migrations live in `UniversityManagementSystem.Infrastructure/Migrations`.
- Migrations are applied automatically on startup (or via `dotnet ef database update`).
- Schema uses the **Instructors** table and `InstructorId` foreign keys.
- A clean database is created from scratch by running the migrations — no existing data dependency.

## Key conventions

- Roles are seeded as `Admin`, `Instructor`, `Student`.
- Login uses **username + password** (not email).
- Student username = their Student UGR/StudentId.
- All naming in code, schema, routes (`/Instructor/*`), and UI follows the **Instructor** terminology.
