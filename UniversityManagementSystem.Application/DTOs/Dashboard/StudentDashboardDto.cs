using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Dashboard
{
    public class StudentDashboardDto
    {
        public string StudentName { get; set; } = "Student";
        public string Greeting { get; set; } = "Good morning";
        public string Subtitle { get; set; } = "Here's your academic overview.";
        public DateTime Today { get; set; }

        public List<DashboardMetric> Metrics { get; set; } = new(); // GPA, Credits, Attendance, Fees

        // My Timetable - Today
        public List<ScheduleItem> Timetable { get; set; } = new();

        // Upcoming Assignments
        public List<AssignmentItem> UpcomingAssignments { get; set; } = new();

        // My Courses grid
        public List<CourseCardItem> Courses { get; set; } = new();

        // Announcements
        public List<AnnouncementItem> Announcements { get; set; } = new();

        // Recent Grades
        public List<GradeItem> RecentGrades { get; set; } = new();
    }
}
