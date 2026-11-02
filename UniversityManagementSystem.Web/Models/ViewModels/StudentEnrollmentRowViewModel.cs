using System;

namespace UniversityManagementSystem.Web.Models.ViewModels
{
    public class StudentEnrollmentRowViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}
