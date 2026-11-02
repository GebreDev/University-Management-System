using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class TimetableSetting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "University Name")]
        public string UniversityName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Start Time")]
        public TimeOnly DayStartTime { get; set; } = new TimeOnly(7, 30);

        [Required]
        [Display(Name = "End Time")]
        public TimeOnly DayEndTime { get; set; } = new TimeOnly(15, 30);

        [Display(Name = "Periods Per Day")]
        public int PeriodsPerDay { get; set; } = 8;

        [Display(Name = "Working Days Per Week")]
        public int WorkingDaysPerWeek { get; set; } = 6;

        [Display(Name = "Lunch Break Start")]
        public TimeOnly? LunchBreakStart { get; set; }

        [Display(Name = "Lunch Break End")]
        public TimeOnly? LunchBreakEnd { get; set; }

        [Display(Name = "Lunch Break Duration (minutes)")]
        public int LunchBreakDurationMinutes { get; set; } = 60;

        [StringLength(50)]
        [Display(Name = "Monday")]
        public string MondayLabel { get; set; } = "Monday";

        [StringLength(50)]
        [Display(Name = "Tuesday")]
        public string TuesdayLabel { get; set; } = "Tuesday";

        [StringLength(50)]
        [Display(Name = "Wednesday")]
        public string WednesdayLabel { get; set; } = "Wednesday";

        [StringLength(50)]
        [Display(Name = "Thursday")]
        public string ThursdayLabel { get; set; } = "Thursday";

        [StringLength(50)]
        [Display(Name = "Friday")]
        public string FridayLabel { get; set; } = "Friday";

        [StringLength(50)]
        [Display(Name = "Saturday")]
        public string SaturdayLabel { get; set; } = "Saturday";

        [Display(Name = "Monday Enabled")]
        public bool MondayEnabled { get; set; } = true;

        [Display(Name = "Tuesday Enabled")]
        public bool TuesdayEnabled { get; set; } = true;

        [Display(Name = "Wednesday Enabled")]
        public bool WednesdayEnabled { get; set; } = true;

        [Display(Name = "Thursday Enabled")]
        public bool ThursdayEnabled { get; set; } = true;

        [Display(Name = "Friday Enabled")]
        public bool FridayEnabled { get; set; } = true;

        [Display(Name = "Saturday Enabled")]
        public bool SaturdayEnabled { get; set; } = false;

        [Display(Name = "Sunday Enabled")]
        public bool SundayEnabled { get; set; } = false;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
