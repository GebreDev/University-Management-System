using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Results
{
    public class CourseAssessmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Weight { get; set; }
    }

    public class AssessmentMarkDto
    {
        public int AssessmentId { get; set; }
        public decimal? Score { get; set; }   // null = no mark entered yet
    }

    public class AssessmentBoardStudentDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public List<AssessmentMarkDto> Marks { get; set; } = new List<AssessmentMarkDto>();
        public decimal TotalMark { get; set; }        // out of 100
        public string LetterGrade { get; set; } = "—";
    }

    /// <summary>
    /// Full model for the instructor assessment entry board (dynamic marks grid).
    /// </summary>
    public class CourseAssessmentBoardDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public decimal Ects { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<CourseAssessmentDto> Assessments { get; set; } = new List<CourseAssessmentDto>();
        public List<AssessmentBoardStudentDto> Students { get; set; } = new List<AssessmentBoardStudentDto>();
    }
}
