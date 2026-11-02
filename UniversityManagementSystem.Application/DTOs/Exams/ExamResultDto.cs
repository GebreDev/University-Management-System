namespace UniversityManagementSystem.Application.DTOs.Exams
{
    public class ExamResultDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int TotalScore { get; set; }
        public string Grade { get; set; }
        public bool IsPass { get; set; }
        public string Remarks { get; set; }
    }
}