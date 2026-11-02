using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Dashboard
{
    /// <summary>Small shared models used by the three role dashboards.</summary>

    public class DashboardMetric
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string SubLabel { get; set; } = string.Empty;
        public string Trend { get; set; } = string.Empty;
        public bool TrendPositive { get; set; } = true;
        public string Icon { get; set; } = string.Empty;
        public string GradientClass { get; set; } = string.Empty;
    }

    public class ChartSlice
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class ActivityItem
    {
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;   // css class used on the icon tile
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }             // internal sort key
    }

    public class EventItem
    {
        public string Day { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Meta { get; set; } = string.Empty;    // e.g. "09:00 AM - Main Auditorium"
        public string ColorClass { get; set; } = string.Empty;
    }

    public class CourseCardItem
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Students { get; set; }
        public string Attendance { get; set; } = string.Empty;  // "92%"
        public string AvgGrade { get; set; } = string.Empty;    // "B+"
        public string Instructor { get; set; } = string.Empty;
        public string NextClass { get; set; } = string.Empty;   // "Today, 8:00 AM"
    }

    public class ScheduleItem
    {
        public string Time { get; set; } = string.Empty;       // "08:00 AM"
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;   // "Room 301"
        public string Meta { get; set; } = string.Empty;       // "42 Students" or instructor name
        public string Progress { get; set; } = string.Empty;   // used by student timetable
    }

    public class TaskItem
    {
        public string Label { get; set; } = string.Empty;
        public string Count { get; set; } = string.Empty;
        public string Meta { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class AssignmentItem
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Due { get; set; } = string.Empty;
        public string Progress { get; set; } = string.Empty;
    }

    public class GradeItem
    {
        public string Title { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }

    public class AnnouncementItem
    {
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
}
