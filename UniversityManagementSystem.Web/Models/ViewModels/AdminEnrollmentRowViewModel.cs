using System;

namespace UniversityManagementSystem.Web.Models.ViewModels
{
    public class AdminEnrollmentRowViewModel
    {
        public int Id { get; set; }
        public int? RegistrationId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}
