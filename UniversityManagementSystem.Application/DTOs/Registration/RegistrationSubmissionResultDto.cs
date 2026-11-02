namespace UniversityManagementSystem.Application.DTOs.Registration
{
    public class RegistrationSubmissionResultDto
    {
        public int RegistrationId { get; set; }
        public int TotalCredits { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
