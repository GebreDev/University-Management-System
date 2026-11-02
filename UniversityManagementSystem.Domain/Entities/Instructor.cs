using UniversityManagementSystem.Domain.Common;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Instructor : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Qualification { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Foreign keys
        public int DepartmentId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;

        // Address foreign keys
        public int? RegionId { get; set; }
        public int? ZoneId { get; set; }
        public int? WoredaId { get; set; }
        public int? KebeleId { get; set; }

        // Navigation properties
        public virtual Department Department { get; set; } = null!;
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
        public virtual Region? Region { get; set; }
        public virtual Zone? Zone { get; set; }
        public virtual Woreda? Woreda { get; set; }
        public virtual Kebele? Kebele { get; set; }
    }
}