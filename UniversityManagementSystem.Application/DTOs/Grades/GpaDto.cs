namespace UniversityManagementSystem.Application.DTOs.Grades
{
    public class GpaDto
    {
        public int StudentId { get; set; }
        public double Gpa { get; set; }
        public int TotalCredits { get; set; }
        public string Semester { get; set; }
    }
}