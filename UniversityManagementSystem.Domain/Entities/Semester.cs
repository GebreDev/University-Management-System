using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Semester
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Semester")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int AcademicYearId { get; set; }

        public AcademicYear? AcademicYear { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
    }
}
