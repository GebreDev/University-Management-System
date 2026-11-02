using UniversityManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Exams
{
    public class ExamDto
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int InstructorId { get; set; }

        [Required]
        public ExamCategory Type { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [Required]
        [Range(1, 480)]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalMarks { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int PassingMarks { get; set; }

        public bool IsPublished { get; set; }
        public bool IsRandomized { get; set; }
        public string Instructions { get; set; }
        public List<QuestionDto> Questions { get; set; }

        // File upload support
        public IFormFile AttachmentFile { get; set; }
        public string? AttachmentPath { get; set; }
    }
}