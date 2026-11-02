using UniversityManagementSystem.Domain.Common;

namespace UniversityManagementSystem.Domain.Entities
{
    public class AssignmentResult : BaseEntity
    {
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public int TotalScore { get; set; }
        public string? Grade { get; set; } = string.Empty;
        public bool IsPass { get; set; }
        public string? Remarks { get; set; } = string.Empty;

        public virtual Student Student { get; set; } = null!;
        public virtual Assignment Assignment { get; set; } = null!;
    }
}
