namespace UniversityManagementSystem.Application.DTOs.Course
{
    public class CourseListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int Credits { get; set; }
        public string InstructorName { get; set; }
        public string DepartmentName { get; set; }
        public int EnrollmentCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}