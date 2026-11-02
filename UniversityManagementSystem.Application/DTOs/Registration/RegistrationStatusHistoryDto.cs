using System;

namespace UniversityManagementSystem.Application.DTOs.Registration
{
    public class RegistrationStatusHistoryDto
    {
        public int Id { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }
        public string? Reason { get; set; }
    }
}
