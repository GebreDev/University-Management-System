using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;
using System;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// Audit trail for registration status changes.
    /// </summary>
    public class RegistrationStatusHistory : BaseEntity
    {
        public int RegistrationId { get; set; }
        public Registration? Registration { get; set; }

        public RegistrationStatus OldStatus { get; set; }
        public RegistrationStatus NewStatus { get; set; }

        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }
        public string? Reason { get; set; }
    }
}
