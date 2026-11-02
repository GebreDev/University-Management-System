using UniversityManagementSystem.Domain.Common;
using System;

namespace UniversityManagementSystem.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        // Optional: audit entries must never fail to insert just because the
        // actor id is unknown (system actions, deleted users, legacy "Admin" ids).
        public string? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string? Details { get; set; } = string.Empty;
        public string? IpAddress { get; set; } = string.Empty;
        public string? DeviceInfo { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
