using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Student : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public string StudentId { get; set; } = string.Empty; // Unique student UGR code
        public string Gender { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        // Foreign keys
        public int DepartmentId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;

        // Address foreign keys
        public int? RegionId { get; set; }
        public int? ZoneId { get; set; }
        public int? WoredaId { get; set; }
        public int? KebeleId { get; set; }

        // Academic foreign keys
        public int? AcademicGradeId { get; set; }
        public int? AcademicSectionId { get; set; }

        // Documents
        public string? ProfilePictureUrl { get; set; }
        public string? StudentDocumentPath { get; set; }

        // Finance: who is financially responsible for this student's tuition.
        // Assigned by the registrar/finance, changeable only by Admin.
        public PaymentCategory PaymentCategory { get; set; } = PaymentCategory.SelfSponsored;

        // Navigation properties
        public virtual Department Department { get; set; } = null!;
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Parent> Parents { get; set; } = new List<Parent>();
        public virtual Region? Region { get; set; }
        public virtual Zone? Zone { get; set; }
        public virtual Woreda? Woreda { get; set; }
        public virtual Kebele? Kebele { get; set; }
        public virtual AcademicGrade? AcademicGrade { get; set; }
        public virtual AcademicSection? AcademicSection { get; set; }
    }
}