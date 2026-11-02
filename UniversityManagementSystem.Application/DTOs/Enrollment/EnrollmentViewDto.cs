using System;

namespace UniversityManagementSystem.Application.DTOs.Enrollment
{
    public class EnrollmentViewDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }          // Foreign key (int)
        public string StudentFullName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;  // Student's unique ID (string)
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}