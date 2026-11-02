using UniversityManagementSystem.Domain.Common;

namespace UniversityManagementSystem.Domain.Entities
{
    public class AssignmentAnswer : BaseEntity
    {
        public int StudentId { get; set; }
        public int AssignmentQuestionId { get; set; }
        public string? AnswerText { get; set; } = string.Empty;
        public int? Score { get; set; }
        public bool IsCorrect { get; set; }

        public virtual Student Student { get; set; } = null!;
        public virtual AssignmentQuestion AssignmentQuestion { get; set; } = null!;
    }
}
