using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UniversityManagementSystem.Application.DTOs.Instructor
{
    public class InstructorCreateViewModel
    {
        // Instructor Information
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-30);

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone")]
        public string Phone { get; set; } = string.Empty;

        // Address Information
        [Display(Name = "Region")]
        public int? RegionId { get; set; }

        [Display(Name = "Zone")]
        public int? ZoneId { get; set; }

        [Display(Name = "Woreda")]
        public int? WoredaId { get; set; }

        [Display(Name = "Kebele")]
        public int? KebeleId { get; set; }

        // Professional Information
        [Required(ErrorMessage = "Qualification is required")]
        public string Qualification { get; set; } = string.Empty;

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }

        [Required(ErrorMessage = "Hire Date is required")]
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Today;

        // Department (required for the service)
        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        // Course Assignments
        public List<InstructorCourseAssignment> CourseAssignments { get; set; } = new List<InstructorCourseAssignment>();

        // Instructor Login
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class InstructorCourseAssignment
    {
        [Display(Name = "Course")]
        public int? CourseId { get; set; }

        [Display(Name = "Year")]
        public int? AcademicGradeId { get; set; }
    }
}
