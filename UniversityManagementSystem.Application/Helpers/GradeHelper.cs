namespace UniversityManagementSystem.Application.Helpers
{
    public static class GradeHelper
    {
        public static string CalculateLetterGrade(int score, int maxScore)
        {
            return CalculateLetterGrade((decimal)score, (decimal)maxScore);
        }

        public static string CalculateLetterGrade(decimal score, decimal maxScore)
        {
            if (maxScore <= 0) return "N/A";
            var percentage = (double)((score / maxScore) * 100m);
            var (letter, _) = ComputeGrade((decimal)percentage);
            return letter;
        }

        public static decimal ComputeGradePoint(decimal totalMark)
        {
            return ComputeGrade(totalMark).Point;
        }

        public static int GetCurrentSemester()
        {
            var now = System.DateTime.UtcNow;
            var year = now.Year;
            var month = now.Month;
            // Academic year: Sep-Feb = Fall (Semester 1), Mar-Aug = Spring (Semester 2)
            if (month >= 9 || month <= 2)
                return month >= 9 ? year * 10 + 1 : (year - 1) * 10 + 1; // Fall Semester
            else
                return year * 10 + 2; // Spring Semester
        }

        public static string GetSemesterName(int semesterCode)
        {
            var year = semesterCode / 10;
            var sem = semesterCode % 10;
            return sem == 1 ? $"Fall {year}" : $"Spring {year}";
        }

        /// <summary>
        /// Academic year label for a semester code, e.g. 20261 => "2025/26".
        /// </summary>
        public static string GetAcademicYearLabel(int semesterCode)
        {
            var year = semesterCode / 10;
            return $"{year - 1}/{year}";
        }

        /// <summary>
        /// Ethiopian university ECTS conversion: 3 credit hours = 5 ECTS.
        /// </summary>
        public static decimal EctsForCredits(int credits)
        {
            return Math.Round((decimal)(credits * 5.0 / 3.0), 2);
        }

        /// <summary>
        /// Full grading scale matching the required breakpoints.
        /// </summary>
        public static (string Letter, decimal Point) ComputeGrade(decimal totalMark)
        {
            if (totalMark >= 90m) return ("A+", 4.00m);
            if (totalMark >= 85m) return ("A", 3.80m);
            if (totalMark >= 80m) return ("A-", 3.50m);
            if (totalMark >= 75m) return ("B+", 3.25m);
            if (totalMark >= 70m) return ("B", 3.00m);
            if (totalMark >= 65m) return ("B-", 2.75m);
            if (totalMark >= 60m) return ("C+", 2.50m);
            if (totalMark >= 55m) return ("C", 2.25m);
            if (totalMark >= 50m) return ("C-", 2.00m);
            if (totalMark >= 40m) return ("D", 1.00m);
            return ("F", 0.00m);
        }

        /// <summary>
        /// Letter grade badge colour per grade value (Bootstrap utility classes).
        /// </summary>
        public static string GradeBadgeClass(string letterGrade)
        {
            switch (letterGrade)
            {
                case "A+":
                case "A":
                case "A-":
                    return "bg-success";
                case "B+":
                case "B":
                case "B-":
                    return "bg-primary";
                case "C+":
                case "C":
                case "C-":
                    return "bg-warning text-dark";
                case "D":
                    return "bg-secondary";
                case "F":
                    return "bg-danger";
                default:
                    return "bg-light text-dark border";
            }
        }
    }
}
