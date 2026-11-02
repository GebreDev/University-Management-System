using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Assignments
{
    public class SubmissionCreateDto
    {
        [Required]
        public int AssignmentId { get; set; }

        public IFormFile File { get; set; }
        public string FileBase64 { get; set; }
        public string FileName { get; set; }
    }
}