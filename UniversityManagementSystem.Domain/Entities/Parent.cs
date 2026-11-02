using UniversityManagementSystem.Domain.Common;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Parent : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string EconomicStatus { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }

        // Foreign keys
        public int StudentId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
