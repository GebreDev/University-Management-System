using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Grades
{
    public class GradeReportDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public List<GradeDto> Grades { get; set; }
        public GpaDto Gpa { get; set; }
        public string Semester { get; set; }
    }
}