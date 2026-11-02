namespace UniversityManagementSystem.Domain.Helpers
{
    public static class SemesterHelper
    {
        public static int GetCurrentSemester()
        {
            var now = System.DateTime.UtcNow;
            var year = now.Year;
            var month = now.Month;
            if (month >= 9 || month <= 2)
                return month >= 9 ? year * 10 + 1 : (year - 1) * 10 + 1;
            return year * 10 + 2;
        }
    }
}
