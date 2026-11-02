using UniversityManagementSystem.Domain.Common;

namespace UniversityManagementSystem.Domain.Entities
{
    public class AssignmentQuestionOption : BaseEntity
    {
        public int AssignmentQuestionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }

        public virtual AssignmentQuestion AssignmentQuestion { get; set; } = null!;
    }
}
