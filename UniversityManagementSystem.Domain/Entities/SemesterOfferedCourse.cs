using UniversityManagementSystem.Domain.Common;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// A course that a department has opened for registration in a specific semester.
    /// Students register against this offering, not directly against the catalog Course.
    /// </summary>
    public class SemesterOfferedCourse : BaseEntity
    {
        public int SemesterId { get; set; }
        public Semester? Semester { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        /// <summary>
        /// Maximum number of students allowed to register for this offering.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// If true, the offering is visible and open for registration.
        /// </summary>
        public bool IsOpen { get; set; } = true;

        public virtual ICollection<OfferedCourseSchedule> Schedules { get; set; } = new List<OfferedCourseSchedule>();
    }
}
