using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    public class AssignmentQuestion : BaseEntity
    {
        public int AssignmentId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public ExamType Type { get; set; }
        public int Marks { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;

        // New MCQ fields
        public string? Explanation { get; set; }
        public string? DifficultyLevel { get; set; } // e.g. Easy, Medium, Hard

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual ICollection<AssignmentQuestionOption> Options { get; set; } = new List<AssignmentQuestionOption>();
    }
}
