using System;

namespace UniversityManagementSystem.Application.DTOs.Enrollment
{
    public class EnrollmentListDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string StudentFullName { get; set; }
        public string StudentId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseCode { get; set; }
        public string Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}