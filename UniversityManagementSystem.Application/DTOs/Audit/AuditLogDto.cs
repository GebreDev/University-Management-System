using System;

namespace UniversityManagementSystem.Application.DTOs.Audit
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string Action { get; set; }
        public string Entity { get; set; }
        public int? EntityId { get; set; }
        public string Details { get; set; }
        public string IpAddress { get; set; }
        public string DeviceInfo { get; set; }
        public DateTime Timestamp { get; set; }
    }
}