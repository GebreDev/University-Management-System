using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Kebele
    {
        public int KebeleId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Kebele")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int WoredaId { get; set; }

        public Woreda? Woreda { get; set; }
    }
}
