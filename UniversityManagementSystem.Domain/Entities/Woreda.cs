using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Woreda
    {
        public int WoredaId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Woreda")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int ZoneId { get; set; }

        public Zone? Zone { get; set; }

        public ICollection<Kebele> Kebeles { get; set; } = new List<Kebele>();
    }
}
