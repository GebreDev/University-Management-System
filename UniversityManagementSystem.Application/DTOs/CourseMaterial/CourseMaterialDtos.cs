using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.CourseMaterial
{
    /// <summary>Row in the material list (instructor or student view).</summary>
    public class CourseMaterialListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int DownloadCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;

        public string UploadedByName { get; set; } = string.Empty;
    }

    /// <summary>Detail view with everything about one material.</summary>
    public class CourseMaterialViewDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int DownloadCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;

        public string UploadedById { get; set; } = string.Empty;
        public string UploadedByName { get; set; } = string.Empty;
    }

    /// <summary>Upload form (file + metadata).</summary>
    public class CourseMaterialUploadDto
    {
        [Required(ErrorMessage = "Please select a course.")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please choose a file to upload.")]
        public IFormFile? File { get; set; }
    }

    /// <summary>Edit form — title/description always; file only when replacing.</summary>
    public class CourseMaterialUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        /// <summary>Optional — when provided, the stored file is replaced.</summary>
        public IFormFile? File { get; set; }
    }
}
