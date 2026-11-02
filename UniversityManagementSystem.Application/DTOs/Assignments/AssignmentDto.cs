using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Assignments
{
    public class AssignmentDto
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        [Required]
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        [Range(1, 1000)]
        public int TotalMarks { get; set; }

        public string AttachmentPath { get; set; }
        public bool IsPublished { get; set; }
        public bool AllowLateSubmission { get; set; }
        public int? LateSubmissionPenalty { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SubmissionsCount { get; set; }
        public int QuestionCount { get; set; }
        public List<AssignmentQuestionDto> Questions { get; set; }
    }
}