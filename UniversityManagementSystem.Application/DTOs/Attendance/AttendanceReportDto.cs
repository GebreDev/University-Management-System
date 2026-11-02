namespace UniversityManagementSystem.Application.DTOs.Attendance
{
    public class AttendanceReportDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Month { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Excused { get; set; }
        public double Percentage { get; set; }
    }
}