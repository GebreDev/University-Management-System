using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Course
{
    public class CourseCreateViewModel
    {
        [Required(ErrorMessage = "Course Name is required")]
        [Display(Name = "Course Name")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course Code is required")]
        [Display(Name = "Course Code")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Credits is required")]
        [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10")]
        public int Credits { get; set; } = 3;

        [Required(ErrorMessage = "Start Date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End Date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(6);

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Instructor is required")]
        [Display(Name = "Instructor")]
        public int InstructorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Semester is required")]
        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        [Display(Name = "Registration Capacity")]
        public int RegistrationCapacity { get; set; } = 30;

        // Year checkboxes (AcademicGrade IDs)
        [Display(Name = "Select Years")]
        public List<int> SelectedYearIds { get; set; } = new List<int>();
    }
}
