using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UniversityManagementSystem.Application.DTOs.Assignments
{
    public class AssignmentUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        [Range(1, 1000)]
        public int TotalMarks { get; set; }

        public bool AllowLateSubmission { get; set; }

        [Range(0, 100)]
        public int? LateSubmissionPenalty { get; set; }

        public string FileBase64 { get; set; }
        public string FileName { get; set; }
        public IFormFile Attachment { get; set; }
    }
}
