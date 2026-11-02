using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityManagementSystem.Domain.Helpers
{
    /// <summary>
    /// Converts the stored academic year (grade) definitions into the class-standing
    /// labels used across the promotion workflow — e.g. year 9 => Freshman,
    /// year 10 => Sophomore, and so on. Keeping the mapping in one place means the
    /// UI never has to hard-code individual year numbers.
    /// </summary>
    public static class YearHelper
    {
        private static readonly string[] ClassStandings =
        {
            "Freshman",
            "Sophomore",
            "Junior",
            "Senior"
        };

        /// <summary>
        /// Class standing for a zero-based year rank: 0 = Freshman, 1 = Sophomore,
        /// 2 = Junior, 3 = Senior. Years beyond the fourth fall back to "Year N" so
        /// newly configured years keep working without any code change.
        /// </summary>
        public static string GetClassStanding(int rank)
        {
            if (rank < 0) rank = 0;

            return rank < ClassStandings.Length
                ? ClassStandings[rank]
                : $"Year {rank + 1}";
        }

        /// <summary>
        /// Builds a lookup of promotion order => class standing for a set of years.
        /// The lowest promotion order (the entry year) is Freshman, the next one is
        /// Sophomore, and so on. Years sharing a promotion order share a standing.
        /// </summary>
        public static IReadOnlyDictionary<int, string> BuildStandingMap(IEnumerable<int> promotionOrders)
        {
            if (promotionOrders == null)
                return new Dictionary<int, string>();

            var ordered = promotionOrders.Distinct().OrderBy(o => o).ToList();
            var map = new Dictionary<int, string>();

            for (int rank = 0; rank < ordered.Count; rank++)
            {
                map[ordered[rank]] = GetClassStanding(rank);
            }

            return map;
        }
    }
}
