using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Instructor
{
    public class InstructorViewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Qualification { get; set; }
        public DateTime HireDate { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public string ApplicationUserId { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<InstructorCourseDto> Courses { get; set; } = new();
    }

    public class InstructorCourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int Credits { get; set; }
    }
}