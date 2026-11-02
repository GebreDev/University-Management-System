using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Dashboard
{
    public class InstructorDashboardDto
    {
        public string InstructorName { get; set; } = "Instructor";
        public string Greeting { get; set; } = "Welcome back";
        public string Subtitle { get; set; } = "Here's your teaching overview for today.";
        public DateTime Today { get; set; }

        public List<DashboardMetric> Metrics { get; set; } = new();

        // Today's Schedule
        public List<ScheduleItem> TodaySchedule { get; set; } = new();

        // Course Performance Overview (radar)
        public List<string> RadarLabels { get; set; } = new();
        public List<double> RadarSeries { get; set; } = new();

        // My Courses grid
        public List<CourseCardItem> Courses { get; set; } = new();

        // Pending Tasks
        public List<TaskItem> PendingTasks { get; set; } = new();

        // Recent Student Activity
        public List<ActivityItem> RecentActivity { get; set; } = new();
    }
}
