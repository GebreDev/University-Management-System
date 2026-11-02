using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Period
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Period Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Start Time")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public TimeOnly EndTime { get; set; }

        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes => (int)(EndTime.ToTimeSpan() - StartTime.ToTimeSpan()).TotalMinutes;

        [StringLength(100)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }
    }
}
