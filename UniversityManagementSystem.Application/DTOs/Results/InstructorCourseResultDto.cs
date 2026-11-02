using System;

namespace UniversityManagementSystem.Application.DTOs.Results
{
    /// <summary>
    /// One row in the instructor "Grades &amp; Results" table (per course).
    /// </summary>
    public class InstructorCourseResultDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public decimal Ects { get; set; }
        public int EnrolledCount { get; set; }
        public decimal AverageMark { get; set; }       // average total % across graded students (0 = none)
        public int GradedCount { get; set; }
        public bool HasDraft { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
