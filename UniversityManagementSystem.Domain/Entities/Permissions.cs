using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    public static class Permissions
    {
        public const string Dashboard = "Dashboard";
        public const string Students = "Students";
        public const string Instructors = "Instructors";
        public const string Courses = "Courses";
        public const string Enrollments = "Enrollments";
        public const string Departments = "Departments";
        public const string Reports = "Reports";
        public const string Attendance = "Attendance";
        public const string Messages = "Messages";
        public const string Schedule = "Schedule";
        public const string Grades = "Grades";
        public const string AcademicYears = "Academic Years";
        public const string AddressManagement = "Address Management";
        public const string UserManagement = "User Management";
        public const string UniversitySettings = "University Settings";
        public const string Notifications = "Notifications";
        public const string AuditLogs = "Audit Logs";

        public static readonly List<string> All = new()
        {
            Dashboard,
            Students,
            Instructors,
            Courses,
            Enrollments,
            Departments,
            Reports,
            Attendance,
            Messages,
            Schedule,
            Grades,
            AcademicYears,
            AddressManagement,
            UserManagement,
            UniversitySettings,
            Notifications,
            AuditLogs
        };
    }
}
