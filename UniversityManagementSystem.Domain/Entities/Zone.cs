using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Zone
    {
        public int ZoneId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Zone")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int RegionId { get; set; }

        public Region? Region { get; set; }

        public ICollection<Woreda> Woredas { get; set; } = new List<Woreda>();
    }
}
