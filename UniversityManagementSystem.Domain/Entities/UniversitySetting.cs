using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class UniversitySetting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "University Name")]
        public string UniversityName { get; set; } = string.Empty;

        [Display(Name = "Region")]
        public int? RegionId { get; set; }
        public Region? Region { get; set; }

        [Display(Name = "Zone")]
        public int? ZoneId { get; set; }
        public Zone? Zone { get; set; }

        [Display(Name = "Woreda")]
        public int? WoredaId { get; set; }
        public Woreda? Woreda { get; set; }

        [Display(Name = "Kebele")]
        public int? KebeleId { get; set; }
        public Kebele? Kebele { get; set; }

        [Display(Name = "Detailed Address")]
        [StringLength(500)]
        public string? DetailedAddress { get; set; }

        [Display(Name = "Phone")]
        [StringLength(30)]
        public string? Phone { get; set; }

        [Display(Name = "Email")]
        [StringLength(150)]
        public string? Email { get; set; }
    }
}
