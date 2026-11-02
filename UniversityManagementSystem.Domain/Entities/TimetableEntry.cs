using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class TimetableEntry
    {
        public int Id { get; set; }

        [Required]
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }

        [Required]
        public int SemesterId { get; set; }
        public Semester? Semester { get; set; }

        [Required]
        public int GradeId { get; set; }
        public AcademicGrade? Grade { get; set; }

        [Required]
        public int SectionId { get; set; }
        public AcademicSection? Section { get; set; }

        [Required]
        [StringLength(20)]
        public string Day { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Period")]
        public int PeriodId { get; set; }
        public Period? Period { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Instructor")]
        public string? Instructor { get; set; }

        [StringLength(50)]
        [Display(Name = "Room")]
        public string? Room { get; set; }
    }
}
