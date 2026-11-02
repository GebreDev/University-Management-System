using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Course
{
    public class CourseViewDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int Credits { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public string InstructorName { get; set; }
        public int InstructorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<CourseEnrollmentDto> Enrollments { get; set; } = new();
    }

    public class CourseEnrollmentDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public string Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}