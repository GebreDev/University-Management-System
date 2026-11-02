using UniversityManagementSystem.Domain.Common;
using System;

namespace UniversityManagementSystem.Domain.Entities
{
    public class CourseSchedule : BaseEntity
    {
        public int CourseId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public virtual Course Course { get; set; } = null!;
    }
}
