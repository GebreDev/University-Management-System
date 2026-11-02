using System;

namespace UniversityManagementSystem.Application.DTOs.Reports
{
    public class ExportFilterDto
    {
        public int? CourseId { get; set; }
        public int? StudentId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ReportType { get; set; } // Students, Instructors, Attendance, Fees, etc.
    }
}