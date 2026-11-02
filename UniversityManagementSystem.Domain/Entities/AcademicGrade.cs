using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class AcademicGrade
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Year")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Promotion Order")]
        public int PromotionOrder { get; set; }

        public ICollection<AcademicSection> Sections { get; set; } = new List<AcademicSection>();
    }
}
