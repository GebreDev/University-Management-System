using UniversityManagementSystem.Domain.Common;
using System;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// A learning resource (PDF, DOC/DOCX, PPT/PPTX, XLS/XLSX, TXT, ZIP or image)
    /// uploaded by an instructor for one of their courses.
    /// </summary>
    public class CourseMaterial : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        /// <summary>Optional description shown to students.</summary>
        public string? Description { get; set; }

        /// <summary>Original file name as uploaded (for display).</summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>Stored path relative to wwwroot, e.g. /uploads/course-materials/xxxx.pdf</summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>File size in bytes (for display and limits).</summary>
        public long FileSize { get; set; }

        /// <summary>Lowercase extension without dot, e.g. "pdf".</summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>Download/view counter for statistics.</summary>
        public int DownloadCount { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

        /// <summary>ApplicationUser id of the uploading instructor (for ownership checks).</summary>
        public string UploadedById { get; set; } = string.Empty;
        public virtual ApplicationUser? UploadedBy { get; set; }
    }
}
