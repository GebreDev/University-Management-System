using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Department
{
    public class DepartmentViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }

        public List<DepartmentInstructorDto> Instructors { get; set; } = new();
        public List<DepartmentStudentDto> Students { get; set; } = new();
        public List<DepartmentCourseDto> Courses { get; set; } = new();
    }

    public class DepartmentInstructorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public class DepartmentStudentDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string StudentId { get; set; }
    }

    public class DepartmentCourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
    }
}