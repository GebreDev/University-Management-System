using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Attendance
{
    public class BulkAttendanceDto
    {
        public int CourseId { get; set; }
        public DateTime Date { get; set; }
        public List<AttendanceCreateDto> Attendances { get; set; }
    }
}