using UniversityManagementSystem.Domain.Common;
using System;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// Schedule entries belonging to a semester offered course.
    /// Used for conflict detection during registration.
    /// </summary>
    public class OfferedCourseSchedule : BaseEntity
    {
        public int SemesterOfferedCourseId { get; set; }
        public SemesterOfferedCourse? SemesterOfferedCourse { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;
    }
}
