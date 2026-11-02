using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Attendance
{
    public class AttendanceStatsDto
    {
        public int TodayPresent { get; set; }
        public int TodayTotal { get; set; }
        public double MonthlyAverage { get; set; }
        public int AbsentToday { get; set; }
        public int FrequentlyAbsentCount { get; set; }
        public List<string> DailyLabels { get; set; }
        public List<int> DailyPresent { get; set; }
    }
}