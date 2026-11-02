namespace UniversityManagementSystem.Application.DTOs.Instructor
{
    public class InstructorListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }
        public int CourseCount { get; set; }
    }
}