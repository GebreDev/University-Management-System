using UniversityManagementSystem.Application.DTOs.Modules;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    /// <summary>
    /// Powers the generic module overview pages (University, Admissions,
    /// settings, ...). Every module key resolves to a real page backed by real
    /// data pulled from the application repositories.
    /// </summary>
    public class ModuleService : IModuleService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IEventRepository _eventRepo;
        private readonly ICourseScheduleRepository _scheduleRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly IExamRepository _examRepo;
        private readonly IGradeRepository _gradeRepo;
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IUserManagementService _userService;

        public ModuleService(
            IStudentRepository studentRepo,
            IInstructorRepository instructorRepo,
            ICourseRepository courseRepo,
            IDepartmentRepository departmentRepo,
            IEnrollmentRepository enrollmentRepo,
            IEventRepository eventRepo,
            ICourseScheduleRepository scheduleRepo,
            INotificationRepository notificationRepo,
            IAssignmentRepository assignmentRepo,
            IExamRepository examRepo,
            IGradeRepository gradeRepo,
            IAttendanceRepository attendanceRepo,
            IUserManagementService userService)
        {
            _studentRepo = studentRepo;
            _instructorRepo = instructorRepo;
            _courseRepo = courseRepo;
            _departmentRepo = departmentRepo;
            _enrollmentRepo = enrollmentRepo;
            _eventRepo = eventRepo;
            _scheduleRepo = scheduleRepo;
            _notificationRepo = notificationRepo;
            _assignmentRepo = assignmentRepo;
            _examRepo = examRepo;
            _gradeRepo = gradeRepo;
            _attendanceRepo = attendanceRepo;
            _userService = userService;
        }

        // =====================================================================
        //  MODULE CATALOG (metadata only — data is computed below)
        // =====================================================================
        private static readonly Dictionary<string, ModuleMeta> Catalog = new()
        {
            // ---- Admin-side modules ----
            ["university"] = new("University", "Institutional overview — departments, programs and people.", "fas fa-university", "stat-grad-a", "Latest"),
            ["academic"] = new("Academic", "Programs, courses and academic structures.", "fas fa-graduation-cap", "stat-grad-b", "Latest courses"),
            ["admissions"] = new("Admissions", "Applications, pending enrollments and onboarding.", "fas fa-file-signature", "stat-grad-c", "Recent enrollments"),
            ["general-settings"] = new("General Settings", "Core system preferences.", "fas fa-sliders", "stat-grad-a", ""),
            ["permissions"] = new("Permissions", "Roles, access control and security policies.", "fas fa-user-shield", "stat-grad-b", ""),
            ["academic-settings"] = new("Academic Settings", "Calendars, grading rules and academic policy.", "fas fa-school", "stat-grad-c", ""),
            ["notification-settings"] = new("Notification Settings", "Alerts, digests and delivery preferences.", "fas fa-bell", "stat-grad-e", ""),
            ["security"] = new("Security", "Sign-in policies, MFA and session control.", "fas fa-shield-halved", "stat-grad-b", ""),

            // ---- Instructor-side modules ----
            ["course-materials"] = new("Course Materials", "Files and resources for your courses.", "fas fa-folder-open", "stat-grad-a", "Your course catalogue"),
            ["calendar"] = new("Calendar", "Your teaching schedule and events.", "fas fa-calendar-days", "stat-grad-d", "Upcoming events"),
            ["announcements"] = new("Announcements", "Communicate with students and staff.", "fas fa-bullhorn", "stat-grad-e", "Your notifications"),
            ["reports-analytics"] = new("Reports & Analytics", "Teaching insights and performance.", "fas fa-chart-line", "stat-grad-a", "Activity"),
            ["theme-display"] = new("Theme & Display", "Appearance and accessibility.", "fas fa-palette", "stat-grad-d", ""),

            // ---- Student-side modules ----
            ["timetable"] = new("Timetable", "Your weekly class schedule.", "fas fa-calendar-week", "stat-grad-b", "Today's classes"),
            ["degree-progress"] = new("Degree Progress", "Track credits toward graduation.", "fas fa-chart-line", "stat-grad-c", "Recent grades"),

            // ---- Shared settings ----
            ["profile-settings"] = new("Profile Settings", "Your personal details and picture.", "fas fa-user-gear", "stat-grad-a", "")
        };

        public async Task<ModuleOverviewDto?> GetModuleOverviewAsync(string moduleKey, string role, string userId)
        {
            if (!Catalog.TryGetValue(moduleKey, out var meta)) return null;

            var dto = new ModuleOverviewDto
            {
                Key = moduleKey,
                Title = meta.Title,
                Subtitle = meta.Subtitle,
                Icon = meta.Icon,
                Gradient = meta.Gradient,
                ItemsTitle = meta.ItemsTitle
            };

            // ---- Base dataset (shared by most modules) ----
            var students = (await _studentRepo.GetAllStudentsWithDetailsAsync()).ToList();
            var instructors = (await _instructorRepo.GetAllInstructorsWithDetailsAsync()).ToList();
            var courses = (await _courseRepo.GetAllCoursesWithDetailsAsync()).ToList();
            var departments = (await _departmentRepo.GetAllDepartmentsWithDetailsAsync()).ToList();
            var enrollments = (await _enrollmentRepo.GetAllEnrollmentsWithDetailsAsync()).ToList();
            var events = (await _eventRepo.GetUpcomingAsync(6)).ToList();

            var now = DateTime.UtcNow;
            var approved = enrollments.Where(e => e.Status == EnrollmentStatus.Approved).ToList();
            var pendingCount = enrollments.Count(e => e.Status == EnrollmentStatus.Pending);
            // Role-specific data
            var myCourses = new List<Course>();
            var myEnrollments = new List<Enrollment>();
            var student = await _userService.GetCurrentStudentAsync();
            var instructor = await _userService.GetCurrentInstructorAsync();

            if (instructor != null)
            {
                myCourses = (await _courseRepo.GetCoursesByInstructorIdAsync(instructor.Id)).ToList();
                myEnrollments = (await _enrollmentRepo.GetEnrollmentsByCourseIdsAsync(myCourses.Select(c => c.Id))).ToList();
            }
            else if (student != null)
            {
                myEnrollments = (await _enrollmentRepo.GetEnrollmentsByStudentIdAsync(student.Id)).ToList();
                myCourses = courses.Where(c => myEnrollments.Any(e => e.CourseId == c.Id)).ToList();
            }

            // ---- Stats per module ----
            var s = new Func<string, string, string, string, string, ModuleStatDto>(
                (label, value, sub, icon, gradient) => new ModuleStatDto { Label = label, Value = value, Sub = sub, Icon = icon, Gradient = gradient });

            var courseCount = myCourses.Count;
            var myStudents = myEnrollments.Where(e => e.Status == EnrollmentStatus.Approved).Select(e => e.StudentId).Distinct().Count();
            var myGrades = new List<Grade>();
            var myAttendance = new List<Attendance>();

            if (student != null)
            {
                myGrades = (await _gradeRepo.GetByStudentIdAsync(student.Id)).ToList();
                myAttendance = (await _attendanceRepo.GetByStudentIdAsync(student.Id)).ToList();
            }
            var myGraded = myGrades.Where(g => g.MaxScore > 0).ToList();
            var gpa = myGraded.Any() ? Math.Round(myGraded.Average(g => (double)g.Score / (double)g.MaxScore * 100.0) / 25.0, 2) : 0;
            var attRate = AttendanceRate(myAttendance);

            switch (moduleKey)
            {
                case "university":
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("Departments", departments.Count.ToString("N0"), "academic units", "fas fa-building", "stat-grad-a"),
                        s("Programs / Courses", courses.Count.ToString("N0"), "offered this year", "fas fa-book-open", "stat-grad-b"),
                        s("Faculty", instructors.Count.ToString("N0"), "across all departments", "fas fa-chalkboard-user", "stat-grad-c"),
                        s("Students", students.Count.ToString("N0"), "enrolled", "fas fa-user-graduate", "stat-grad-d")
                    };
                    dto.Items = events.Take(4).Select(EventItem).ToList();
                    break;

                case "academic":
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("Departments", departments.Count.ToString("N0"), "offering programs", "fas fa-building", "stat-grad-a"),
                        s("Courses", courses.Count.ToString("N0"), "in catalogue", "fas fa-book-open", "stat-grad-b"),
                        s("Active Enrollments", approved.Count.ToString("N0"), "approved seats", "fas fa-user-check", "stat-grad-c"),
                        s("Students", students.Count.ToString("N0"), "enrolled", "fas fa-user-graduate", "stat-grad-d")
                    };
                    dto.Items = courses.OrderByDescending(c => c.CreatedAt).Take(4).Select(c => new ModuleItemDto
                    {
                        Title = c.Title,
                        Meta = $"{c.Code} · {c.Credits} credits",
                        Date = c.CreatedAt.ToString("MMM dd"),
                        Icon = "fas fa-book",
                        Color = "primary",
                        Url = "/Admin/Courses"
                    }).ToList();
                    break;

                case "admissions":
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("Pending Applications", pendingCount.ToString("N0"), "awaiting review", "fas fa-clock", "stat-grad-c"),
                        s("Total Enrollments", enrollments.Count.ToString("N0"), "all time", "fas fa-list-check", "stat-grad-a"),
                        s("Approved", approved.Count.ToString("N0"), "active seats", "fas fa-check-circle", "stat-grad-b"),
                        s("Programs", courses.Count.ToString("N0"), "to choose from", "fas fa-graduation-cap", "stat-grad-d")
                    };
                    dto.Items = enrollments.OrderByDescending(e => e.EnrollmentDate).Take(4).Select(e => new ModuleItemDto
                    {
                        Title = $"{e.Student?.FirstName} {e.Student?.LastName}",
                        Meta = e.Course?.Title ?? "",
                        Date = e.EnrollmentDate.ToString("MMM dd"),
                        Icon = "fas fa-user-plus",
                        Color = e.Status == EnrollmentStatus.Pending ? "warning" : "success",
                        Url = "/Admin/Enrollments"
                    }).ToList();
                    break;



                // ---- Settings modules (system-wide health snapshot) ----
                case "general-settings":
                case "permissions":
                case "academic-settings":
                case "notification-settings":
                case "security":
                case "theme-display":
                case "profile-settings":
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("Users", (await _userService.GetPagedUsersAsync(null, null, 1, 1)).TotalRecords.ToString("N0"), "accounts", "fas fa-users", "stat-grad-a"),
                        s("Departments", departments.Count.ToString("N0"), "configured", "fas fa-building", "stat-grad-b"),
                        s("Courses", courses.Count.ToString("N0"), "active", "fas fa-book-open", "stat-grad-c"),
                        s("Events", events.Count.ToString(), "upcoming", "fas fa-calendar-days", "stat-grad-d")
                    };
                    break;

                // ---- Instructor modules ----
                case "course-materials":
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("My Courses", courseCount.ToString(), "I teach", "fas fa-book-open", "stat-grad-a"),
                        s("Assignments", (await _assignmentRepo.GetByInstructorIdAsync(instructor?.Id ?? 0)).Count().ToString(), "created", "fas fa-tasks", "stat-grad-b"),
                        s("Exams", (await _examRepo.GetByInstructorIdAsync(instructor?.Id ?? 0)).Count().ToString(), "created", "fas fa-file-alt", "stat-grad-c"),
                        s("Students", myStudents.ToString("N0"), "across my courses", "fas fa-user-graduate", "stat-grad-d")
                    };
                    dto.Items = myCourses.Take(4).Select(c => new ModuleItemDto
                    {
                        Title = c.Title,
                        Meta = $"{c.Code} · {c.Credits} credits",
                        Date = c.CreatedAt.ToString("MMM dd"),
                        Icon = "fas fa-book",
                        Color = "primary",
                        Url = "/Instructor/Courses"
                    }).ToList();
                    break;

                case "reports-analytics":
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("My Courses", courseCount.ToString(), "currently teaching", "fas fa-book-open", "stat-grad-b"),
                        s("My Students", myStudents.ToString("N0"), "enrolled", "fas fa-user-graduate", "stat-grad-a"),
                        s("Enrollments", myEnrollments.Count.ToString("N0"), "total seats", "fas fa-list-check", "stat-grad-c"),
                        s("Upcoming Events", events.Count.ToString(), "on campus", "fas fa-calendar-days", "stat-grad-d")
                    };
                    dto.Items = myCourses.Take(4).Select(c => new ModuleItemDto
                    {
                        Title = c.Title,
                        Meta = $"{c.Code} · {c.Credits} credits",
                        Date = c.CreatedAt.ToString("MMM dd"),
                        Icon = "fas fa-book",
                        Color = "info",
                        Url = "/Instructor/Courses"
                    }).ToList();
                    break;

                case "calendar":
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("My Courses", courseCount.ToString(), "scheduled", "fas fa-book-open", "stat-grad-d"),
                        s("Classes This Week", (await _scheduleRepo.GetByCourseIdsAsync(myCourses.Select(c => c.Id))).Count().ToString(), "sessions", "fas fa-calendar-week", "stat-grad-a"),
                        s("My Students", myStudents.ToString("N0"), "across courses", "fas fa-user-graduate", "stat-grad-b"),
                        s("Upcoming Events", events.Count.ToString(), "on campus", "fas fa-calendar-days", "stat-grad-c")
                    };
                    dto.Items = events.Take(4).Select(EventItem).ToList();
                    break;

                case "announcements":
                    var myNotifs = (await _notificationRepo.GetByUserIdAsync(userId)).ToList();
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("Total Notifications", myNotifs.Count.ToString(), "for you", "fas fa-bell", "stat-grad-e"),
                        s("Unread", myNotifs.Count(n => !n.IsRead).ToString(), "awaiting attention", "fas fa-envelope-open", "stat-grad-a"),
                        s("Announcements", myNotifs.Count(n => n.Type == "Announcement").ToString(), "published", "fas fa-bullhorn", "stat-grad-b"),
                        s("Upcoming Events", events.Count.ToString(), "on campus", "fas fa-calendar-days", "stat-grad-c")
                    };
                    dto.Items = myNotifs.OrderByDescending(n => n.CreatedAt).Take(4).Select(n => new ModuleItemDto
                    {
                        Title = n.Title,
                        Meta = n.Message,
                        Date = n.CreatedAt.ToString("MMM dd"),
                        Icon = "fas fa-bell",
                        Color = n.IsRead ? "primary" : "warning",
                        Url = "/Notification/Index"
                    }).ToList();
                    break;

                // ---- Student modules ----
                case "degree-progress":
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("GPA", gpa.ToString("F2"), "out of 4.00", "fas fa-star", "stat-grad-c"),
                        s("Courses", myCourses.Count.ToString(), "enrolled", "fas fa-book-open", "stat-grad-a"),
                        s("Credits", myCourses.Sum(c => c.Credits).ToString(), "currently enrolled", "fas fa-layer-group", "stat-grad-b"),
                        s("Attendance", attRate + "%", "this semester", "fas fa-calendar-check", "stat-grad-d")
                    };
                    dto.Items = myCourses.Take(4).Select(c => new ModuleItemDto
                    {
                        Title = c.Title,
                        Meta = $"{c.Code} · {c.Credits} credits",
                        Date = c.CreatedAt.ToString("MMM dd"),
                        Icon = "fas fa-book",
                        Color = "primary",
                        Url = "/Student/Enrollments"
                    }).ToList();
                    break;

                case "timetable":
                    var mySchedule = (await _scheduleRepo.GetByCourseIdsAsync(myCourses.Select(c => c.Id))).ToList();
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("Courses", myCourses.Count.ToString(), "scheduled", "fas fa-book-open", "stat-grad-b"),
                        s("Classes This Week", mySchedule.Count.ToString(), "sessions", "fas fa-calendar-week", "stat-grad-a"),
                        s("Attendance", attRate + "%", "this semester", "fas fa-calendar-check", "stat-grad-d"),
                        s("GPA", gpa.ToString("F2"), "out of 4.00", "fas fa-star", "stat-grad-c")
                    };
                    dto.Items = mySchedule.GroupBy(sch => sch.DayOfWeek).OrderBy(g => (int)g.Key)
                        .Take(5)
                        .SelectMany(g => g.OrderBy(sch => sch.StartTime).Take(2))
                        .Select(sch => new ModuleItemDto
                        {
                            Title = sch.Course?.Title ?? "",
                            Meta = $"{sch.DayOfWeek} · {FormatTime(sch.StartTime)} · {sch.Room}",
                            Date = FormatTime(sch.StartTime),
                            Icon = "fas fa-clock",
                            Color = "info",
                            Url = "/Student/MyAttendance"
                        }).ToList();
                    break;

                default:
                    dto.Stats = new List<ModuleStatDto>
                    {
                        s("Students", students.Count.ToString("N0"), "enrolled", "fas fa-user-graduate", "stat-grad-a"),
                        s("Courses", courses.Count.ToString("N0"), "active", "fas fa-book-open", "stat-grad-b"),
                        s("Departments", departments.Count.ToString("N0"), "units", "fas fa-building", "stat-grad-c"),
                        s("Upcoming Events", events.Count.ToString(), "on campus", "fas fa-calendar-days", "stat-grad-d")
                    };
                    break;
            }

            return dto;
        }

        // =====================================================================
        //  HELPERS
        // =====================================================================
        private static ModuleItemDto EventItem(UniversityEvent e) => new()
        {
            Title = e.Title,
            Meta = $"{e.StartDateTime:hh:mm tt} · {e.Location}",
            Date = $"{e.StartDateTime:MMM} {e.StartDateTime:dd}",
            Icon = "fas fa-calendar-days",
            Color = string.IsNullOrWhiteSpace(e.Color) ? "primary" : e.Color,
            Url = ""
        };

        private static int AttendanceRate(IEnumerable<Attendance> attendance)
        {
            var list = attendance.ToList();
            if (!list.Any()) return 0;
            var present = list.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Excused);
            return (int)Math.Round(present * 100.0 / list.Count);
        }

        private static string FormatTime(TimeSpan t) => DateTime.Today.Add(t).ToString("hh:mm tt");

        private sealed record ModuleMeta(string Title, string Subtitle, string Icon, string Gradient, string ItemsTitle);
    }
}
