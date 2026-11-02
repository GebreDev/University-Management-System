using UniversityManagementSystem.Application.DTOs.Modules;
using UniversityManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    /// <summary>
    /// Serves the generic module overview pages (University, Admissions,
    /// and the settings sub-pages). Every module key in the catalog resolves to a
    /// real page with real data and quick links back to the full CRUD pages of the
    /// application.
    /// </summary>
    [Authorize]
    public class ModuleController : Controller
    {
        private readonly IModuleService _moduleService;
        private readonly IUserManagementService _userService;

        public ModuleController(IModuleService moduleService, IUserManagementService userService)
        {
            _moduleService = moduleService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(string module)
        {
            var user = await _userService.GetCurrentUserAsync();
            var role = await _userService.GetUserRoleAsync() ?? "";
            var dashboardController = role == "Admin" ? "Admin" : role == "Instructor" ? "Instructor" : "Student";

            if (string.IsNullOrWhiteSpace(module))
                return RedirectToAction("Dashboard", dashboardController);

            var model = await _moduleService.GetModuleOverviewAsync(module, role, user.Id);
            if (model == null)
                return NotFound();

            model.QuickLinks = BuildQuickLinks(module, role);
            ViewData["Title"] = model.Title;
            return View(model);
        }

        // =====================================================================
        //  Quick links: label + icon + real app route
        // =====================================================================
        private List<ModuleLinkDto> BuildQuickLinks(string module, string role)
        {
            var links = new List<ModuleLinkDto>();

            void Add(string label, string icon, string controller, string action, string description = "")
                => links.Add(new ModuleLinkDto
                {
                    Label = label,
                    Icon = icon,
                    Url = Url.Action(action, controller) ?? "#",
                    Description = description
                });

            switch (module)
            {
                // ---------- Admin modules ----------
                case "university":
                    Add("Departments", "fas fa-building", "Admin", "Departments", "Manage academic departments");
                    Add("Courses", "fas fa-book-open", "Admin", "Courses", "Browse the course catalogue");
                    Add("Instructors", "fas fa-chalkboard-user", "Admin", "Instructors", "Manage faculty");
                    Add("Students", "fas fa-user-graduate", "Admin", "Students", "Manage student records");
                    break;
                case "academic":
                    Add("Courses", "fas fa-book-open", "Admin", "Courses", "Browse all programs & courses");
                    Add("Departments", "fas fa-building", "Admin", "Departments", "Academic departments");
                    Add("Enrollments", "fas fa-list-check", "Admin", "Enrollments", "All enrollment records");
                    Add("Attendance Overview", "fas fa-calendar-check", "Admin", "AttendanceOverview", "Institution-wide attendance");
                    break;
                case "admissions":
                    Add("Enrollments", "fas fa-list-check", "Admin", "Enrollments", "All enrollment records");
                    Add("Students", "fas fa-user-graduate", "Admin", "Students", "Manage student records");
                    break;
                case "general-settings":
                case "permissions":
                    Add("Users & Roles", "fas fa-users", "Admin", "Users", "Manage accounts & permissions");
                    Add("My Profile", "fas fa-user-circle", "Account", "Profile", "Personal details");
                    break;
                case "academic-settings":
                    Add("Departments", "fas fa-building", "Admin", "Departments", "Configure departments");
                    Add("Courses", "fas fa-book-open", "Admin", "Courses", "Configure programs");
                    break;

                case "notification-settings":
                    Add("My Notifications", "fas fa-bell", "Notification", "Index", "View all notifications");
                    break;
                case "security":
                    Add("Users & Roles", "fas fa-users", "Admin", "Users", "Manage access");
                    Add("Audit Logs", "fas fa-history", "Admin", "AuditLogs", "Security trail");
                    break;
                // ---------- Instructor modules ----------
                case "course-materials":
                    Add("My Courses", "fas fa-book-open", "Instructor", "Courses", "Your course catalogue");
                    Add("Assignments", "fas fa-tasks", "Instructor", "Assignments", "Assignment materials");
                    Add("Exams", "fas fa-file-alt", "Instructor", "Exams", "Examination materials");
                    break;
                case "calendar":
                    Add("My Courses", "fas fa-book-open", "Instructor", "Courses", "Scheduled courses");
                    Add("Attendance", "fas fa-calendar-check", "Instructor", "Courses", "Class sessions");
                    break;
                case "announcements":
                    Add("My Notifications", "fas fa-bell", "Notification", "Index", "View all notifications");
                    Add("Messages", "fas fa-comments", "Chat", "Index", "Chat with students");
                    break;
                case "reports-analytics":
                    Add("My Courses", "fas fa-book-open", "Instructor", "Courses", "Course overview");
                    Add("Grades & Results", "fas fa-star", "Instructor", "Grades", "Performance analytics");
                    Add("My Students", "fas fa-user-graduate", "Instructor", "Courses", "Student insights");
                    break;
                case "theme-display":
                    Add("My Profile", "fas fa-user-circle", "Instructor", "Profile", "Personal details");
                    Add("Password & Security", "fas fa-key", "Account", "ChangePassword", "Update your password");
                    break;

                // ---------- Student modules ----------
                case "timetable":
                    Add("My Attendance", "fas fa-calendar-check", "Student", "MyAttendance", "Your class record");
                    Add("My Enrollments", "fas fa-list-check", "Student", "Enrollments", "Your courses");
                    break;
                case "degree-progress":
                    Add("Grades & Results", "fas fa-star", "Student", "MyGrades", "Grade history");
                    Add("My Enrollments", "fas fa-list-check", "Student", "Enrollments", "Credit plan");
                    break;
                case "profile-settings":
                    Add("My Profile", "fas fa-user-circle", "Account", "Profile", "Personal details");
                    Add("Password & Security", "fas fa-key", "Account", "ChangePassword", "Update your password");
                    break;

                default:
                    break;
            }

            // Fallback: always give the user a way back to their dashboard
            var dashboardController = role == "Admin" ? "Admin" : role == "Instructor" ? "Instructor" : "Student";
            links.Insert(0, new ModuleLinkDto
            {
                Label = "Dashboard",
                Icon = "fas fa-tachometer-alt",
                Url = Url.Action("Dashboard", dashboardController) ?? "#",
                Description = "Back"
            });

            return links;
        }
    }
}
