using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Registration
{
    public class RegistrationDetailDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalCredits { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectionReason { get; set; }
        public List<RegistrationItemDto> Items { get; set; } = new();
        public List<RegistrationStatusHistoryDto> StatusHistory { get; set; } = new();
    }
}
