namespace UniversityManagementSystem.Application.DTOs.Promotion
{
    public class PromotionSectionDto
    {
        public int GradeId { get; set; }

        /// <summary>Stored year label as configured under Timetable &gt; Years (e.g. "9").</summary>
        public string YearName { get; set; } = string.Empty;

        /// <summary>Class standing shown to users (e.g. "Freshman"), mapped from the year rank.</summary>
        public string YearDisplayName { get; set; } = string.Empty;

        public int PromotionOrder { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int PendingSemester1 { get; set; }
        public int PendingSemester2 { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsGraduating { get; set; }
    }

    public class PromotionOverviewDto
    {
        public int CurrentAcademicYearId { get; set; }
        public string CurrentAcademicYearName { get; set; } = string.Empty;
        public List<PromotionSectionDto> Sections { get; set; } = new();
        public int TotalStudents { get; set; }
        public int TotalEligible { get; set; }
        public int TotalGraduating { get; set; }
    }
}
