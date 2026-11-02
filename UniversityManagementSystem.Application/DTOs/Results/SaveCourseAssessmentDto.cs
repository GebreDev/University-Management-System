using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Results
{
    /// <summary>
    /// Payload submitted from the instructor assessment board. Columns are dynamic,
    /// so each assessment (and every mark) carries a client-generated key that the
    /// service uses to group marks under the correct new column.
    /// </summary>
    public class SaveCourseAssessmentDto
    {
        public int CourseId { get; set; }
        public List<CourseAssessmentInputDto> Assessments { get; set; } = new List<CourseAssessmentInputDto>();
        public List<AssessmentMarkInputDto> Marks { get; set; } = new List<AssessmentMarkInputDto>();
    }

    public class CourseAssessmentInputDto
    {
        public string Key { get; set; } = string.Empty;   // client key (stable within the request)
        public string Name { get; set; } = string.Empty;
        public decimal Weight { get; set; }
    }

    public class AssessmentMarkInputDto
    {
        public string AssessmentKey { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public decimal Score { get; set; }
    }
}
