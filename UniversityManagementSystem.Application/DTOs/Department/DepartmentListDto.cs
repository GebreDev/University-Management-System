namespace UniversityManagementSystem.Application.DTOs.Department
{
    public class DepartmentListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int InstructorCount { get; set; }
        public int StudentCount { get; set; }
        public int CourseCount { get; set; }
    }
}