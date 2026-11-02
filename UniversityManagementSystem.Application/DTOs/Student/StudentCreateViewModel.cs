using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UniversityManagementSystem.Application.DTOs.Student
{
    public class StudentCreateViewModel
    {
        // Student Information
        [Required(ErrorMessage = "Student UGR is required")]
        [Display(Name = "Student UGR")]
        public string StudentId { get; set; } = string.Empty;

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

        [Required(ErrorMessage = "Nationality is required")]
        public string Nationality { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-18);

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

        // Academic Information
        [Display(Name = "Year")]
        public int? AcademicGradeId { get; set; }

        [Display(Name = "Class")]
        public int? AcademicSectionId { get; set; }

        // Student Documents
        [Display(Name = "Profile Photo")]
        public IFormFile? ProfilePhoto { get; set; }

        [Display(Name = "Student Document")]
        public IFormFile? StudentDocument { get; set; }

        // Student Login
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // ===== Parent Registration (optional) =====
        public bool RegisterParent { get; set; } = false;

        [Display(Name = "Parent First Name")]
        public string ParentFirstName { get; set; } = string.Empty;

        [Display(Name = "Parent Middle Name")]
        public string ParentMiddleName { get; set; } = string.Empty;

        [Display(Name = "Parent Last Name")]
        public string ParentLastName { get; set; } = string.Empty;

        [Display(Name = "Parent Username")]
        public string ParentUsername { get; set; } = string.Empty;

        [Display(Name = "Parent Email")]
        [EmailAddress(ErrorMessage = "Invalid parent email address")]
        public string ParentEmail { get; set; } = string.Empty;

        [Display(Name = "Parent Phone")]
        public string ParentPhone { get; set; } = string.Empty;

        [Display(Name = "Parent Nationality")]
        public string ParentNationality { get; set; } = string.Empty;

        [Display(Name = "Economic Status")]
        public string EconomicStatus { get; set; } = string.Empty;

        [Display(Name = "Parent Profile Photo")]
        public IFormFile? ParentProfilePhoto { get; set; }

        [Display(Name = "Parent Password")]
        [DataType(DataType.Password)]
        public string ParentPassword { get; set; } = string.Empty;

        [Display(Name = "Confirm Parent Password")]
        [DataType(DataType.Password)]
        [Compare("ParentPassword", ErrorMessage = "Parent passwords do not match")]
        public string ConfirmParentPassword { get; set; } = string.Empty;
    }
}
