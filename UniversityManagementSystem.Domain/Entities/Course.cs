using UniversityManagementSystem.Domain.Common;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Course : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Credits { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Foreign keys
        public int DepartmentId { get; set; }
        public int InstructorId { get; set; }

        // Navigation properties
        public virtual Department Department { get; set; } = null!;
        public virtual Instructor Instructor { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<CourseGrade> CourseGrades { get; set; } = new List<CourseGrade>();
    }
}