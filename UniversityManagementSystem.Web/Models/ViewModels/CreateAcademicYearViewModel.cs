using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Web.Models.ViewModels
{
    public class CreateAcademicYearViewModel
    {
        [Required(ErrorMessage = "Academic year name is required.")]
        [StringLength(20)]
        [Display(Name = "Academic Year")]
        public string YearName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required.")]
        [Display(Name = "Academic Year Start Date")]
        public DateTime YearStartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End date is required.")]
        [Display(Name = "Academic Year End Date")]
        public DateTime YearEndDate { get; set; } = DateTime.Today.AddYears(1);

        // Semester 1
        [Required(ErrorMessage = "Semester 1 start date is required.")]
        [Display(Name = "Semester 1 Start Date")]
        public DateTime Semester1StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Semester 1 end date is required.")]
        [Display(Name = "Semester 1 End Date")]
        public DateTime Semester1EndDate { get; set; } = DateTime.Today.AddMonths(6);

        // Semester 2
        [Required(ErrorMessage = "Semester 2 start date is required.")]
        [Display(Name = "Semester 2 Start Date")]
        public DateTime Semester2StartDate { get; set; } = DateTime.Today.AddMonths(6);

        [Required(ErrorMessage = "Semester 2 end date is required.")]
        [Display(Name = "Semester 2 End Date")]
        public DateTime Semester2EndDate { get; set; } = DateTime.Today.AddYears(1);

        [Display(Name = "Current Academic Year")]
        public bool IsCurrent { get; set; }
    }
}
