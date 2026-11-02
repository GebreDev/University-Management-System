using UniversityManagementSystem.Domain.Common;

namespace UniversityManagementSystem.Domain.Entities
{
    public class ExamResult : BaseEntity
    {
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public int TotalScore { get; set; }
        public string? Grade { get; set; } = string.Empty;
        public bool IsPass { get; set; }
        public string? Remarks { get; set; } = string.Empty;

        public virtual Student Student { get; set; } = null!;
        public virtual Exam Exam { get; set; } = null!;
    }
}