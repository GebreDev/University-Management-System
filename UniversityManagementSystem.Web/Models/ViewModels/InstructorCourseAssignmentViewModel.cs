namespace UniversityManagementSystem.Web.Models.ViewModels
{
    public class InstructorCourseAssignmentViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int EnrollmentCount { get; set; }
        public int AssignmentCount { get; set; }
        public int ExamCount { get; set; }
        public int PendingSubmissions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Semester { get; set; } = string.Empty;
    }
}
