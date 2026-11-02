using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// A student's registration submission for one or more semester offered courses.
    /// This represents a semester course registration transaction, not a single course enrollment.
    /// </summary>
    public class Registration : BaseEntity
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int SemesterId { get; set; }
        public Semester? Semester { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public new RegistrationStatus Status { get; set; } = RegistrationStatus.Draft;

        /// <summary>
        /// Total credits across the registered offered courses.
        /// </summary>
        public int TotalCredits { get; set; }

        public string? RejectionReason { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; }

        public virtual ICollection<RegistrationItem> Items { get; set; } = new List<RegistrationItem>();
        public virtual ICollection<RegistrationStatusHistory> StatusHistory { get; set; } = new List<RegistrationStatusHistory>();
    }
}
