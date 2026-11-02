using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Dashboard
{
    public class AdminDashboardDto
    {
        public string AdminName { get; set; } = "Administrator";
        public string Greeting { get; set; } = "Good morning";
        public string Subtitle { get; set; } = "Here's what's happening across the university today.";
        public DateTime Today { get; set; }
        public int PendingCourseRegistrations { get; set; }

        public List<DashboardMetric> Metrics { get; set; } = new();

        // Enrollment Overview (grouped bars)
        public List<string> Months { get; set; } = new();
        public List<int> ThisSemesterEnrollments { get; set; } = new();
        public List<int> LastSemesterEnrollments { get; set; } = new();

        // Student Distribution (doughnut)
        public List<ChartSlice> StudentDistribution { get; set; } = new();

        // Academic Performance (line)
        public List<string> PerfMonths { get; set; } = new();
        public List<double> GpaSeries { get; set; } = new();
        public List<double> PassRateSeries { get; set; } = new();
        public List<double> RetentionSeries { get; set; } = new();

        public List<ActivityItem> RecentActivities { get; set; } = new();
        public List<EventItem> UpcomingEvents { get; set; } = new();
    }
}
