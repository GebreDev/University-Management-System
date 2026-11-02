using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Results
{
    public class CourseAssessmentResultDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
    }

    /// <summary>
    /// Breakdown of a single course result for a student (shown in the assessment modal).
    /// </summary>
    public class StudentCourseResultDetailDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public decimal TotalMark { get; set; }        // out of 100
        public string LetterGrade { get; set; } = "—";
        public decimal GradePoint { get; set; }
        public List<CourseAssessmentResultDto> Assessments { get; set; } = new List<CourseAssessmentResultDto>();
    }
}
