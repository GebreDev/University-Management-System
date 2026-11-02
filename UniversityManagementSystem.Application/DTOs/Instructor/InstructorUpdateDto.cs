using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Instructor
{
    public class InstructorUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string Qualification { get; set; }
        public int DepartmentId { get; set; }
    }
}