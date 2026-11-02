namespace UniversityManagementSystem.Application.DTOs.Registration
{
    public class RegistrationItemDto
    {
        public int Id { get; set; }
        public int SemesterOfferedCourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
