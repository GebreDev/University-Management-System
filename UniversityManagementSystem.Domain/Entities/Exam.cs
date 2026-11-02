using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Exam : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
        public ExamCategory Type { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public bool IsPublished { get; set; }
        public bool IsRandomized { get; set; }
        public string? Instructions { get; set; } = string.Empty;
        public string? AttachmentPath { get; set; } = string.Empty;

        public virtual Course Course { get; set; } = null!;
        public virtual Instructor Instructor { get; set; } = null!;
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}