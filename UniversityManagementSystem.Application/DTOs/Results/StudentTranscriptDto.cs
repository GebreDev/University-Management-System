using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Results
{
    public class StudentTranscriptRowDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public decimal Ects { get; set; }
        public decimal TotalMark { get; set; }        // out of 100
        public string LetterGrade { get; set; } = "—";
        public decimal GradePoint { get; set; }
    }

    /// <summary>
    /// Student "Grades &amp; Results" transcript with SGP / CGP / SGPA / CGPA summary.
    /// </summary>
    public class StudentTranscriptDto
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;    // e.g. "2025/26"
        public string SemesterLabel { get; set; } = string.Empty;   // e.g. "Semester One"
        public string YearLabel { get; set; } = string.Empty;       // e.g. "Year III"
        public int? SelectedSemester { get; set; }

        /// <summary>
        /// Semester codes the student actually has published results for (e.g. 20251 = Fall 2025).
        /// </summary>
        public List<int> AvailableSemesters { get; set; } = new List<int>();

        public decimal Sgp { get; set; }
        public decimal Cgp { get; set; }
        public decimal Sgpa { get; set; }
        public decimal Cgpa { get; set; }
        public string AcademicStatus { get; set; } = "—";

        public List<StudentTranscriptRowDto> Rows { get; set; } = new List<StudentTranscriptRowDto>();
    }
}
