namespace UniversityManagementSystem.Web.Models
{
    /// <summary>
    /// Per-course attendance summary shown on the instructor Attendance overview page.
    /// </summary>
    public class InstructorAttendanceViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = "";
        public string CourseCode { get; set; } = "";
        public int TotalRecords { get; set; }
        public int TodayRecords { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int ExcusedCount { get; set; }

        public double AttendanceRate => TotalRecords == 0 ? 0 : (double)PresentCount / TotalRecords * 100;

        /// <summary>Whether attendance has already been recorded for today.</summary>
        public bool MarkedToday => TodayRecords > 0;
    }
}
