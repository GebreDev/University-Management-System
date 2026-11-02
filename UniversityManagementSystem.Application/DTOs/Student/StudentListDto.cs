namespace UniversityManagementSystem.Application.DTOs.Student
{
    public class StudentListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string StudentId { get; set; }
        public string DepartmentName { get; set; }
        public int EnrollmentCount { get; set; }
    }
}