using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Student
{
    public class StudentViewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string StudentId { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public string ApplicationUserId { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<StudentEnrollmentDto> Enrollments { get; set; } = new();
    }

    public class StudentEnrollmentDto
    {
        public int Id { get; set; }
        public string CourseTitle { get; set; }
        public string CourseCode { get; set; }
        public string Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}