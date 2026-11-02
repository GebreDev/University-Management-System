namespace UniversityManagementSystem.Application.DTOs.Student
{
    public class StudentAcademicRecordDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentUgr { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalCourses { get; set; }
        public int ApprovedEnrollments { get; set; }
        public int PendingEnrollments { get; set; }
    }
}
