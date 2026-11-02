using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class CourseGrade
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int GradeId { get; set; }
        public AcademicGrade Grade { get; set; } = null!;
    }
}
