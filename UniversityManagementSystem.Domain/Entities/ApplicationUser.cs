using UniversityManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }  // <-- Made nullable with '?'
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Navigation properties
        public virtual Student? Student { get; set; }
        public virtual Instructor? Instructor { get; set; }
        public virtual Parent? Parent { get; set; }
    }
}