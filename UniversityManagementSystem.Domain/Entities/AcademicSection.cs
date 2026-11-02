using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class AcademicSection
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Section")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int GradeId { get; set; }

        public AcademicGrade? Grade { get; set; }

        public int? InstructorId { get; set; }

        public Instructor? Instructor { get; set; }
    }
}
