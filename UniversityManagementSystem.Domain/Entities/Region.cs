using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Region
    {
        public int RegionId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Region")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Zone> Zones { get; set; } = new List<Zone>();
    }
}
