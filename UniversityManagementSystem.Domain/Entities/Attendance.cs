using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;
using System;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Attendance : BaseEntity
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime Date { get; set; }
        public new AttendanceStatus Status { get; set; }
        public string? Remarks { get; set; } = string.Empty;
        public int? InstructorId { get; set; }

        public virtual Student Student { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
        public virtual Instructor Instructor { get; set; } = null!;
    }
}