using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class AcademicYear
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Academic Year")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);

        public bool IsCurrent { get; set; }
    }
}
