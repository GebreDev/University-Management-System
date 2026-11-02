using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IFileService
    {
        Task<string?> SaveFileAsync(string base64Content, string folder, string? fileName = null);
        Task<string?> SaveFileAsync(IFormFile file, string folder);

        /// <summary>
        /// Saves a course material file (PDF, DOC/DOCX, PPT/PPTX, XLS/XLSX, TXT,
        /// ZIP, images) into /uploads/course-materials with extension validation.
        /// </summary>
        void DeleteFile(string relativePath);
        Task<byte[]?> GetFileAsync(string relativePath);
        Task<string?> SaveProfilePictureAsync(IFormFile file, string userId);
        Task<string?> SaveMaterialFileAsync(IFormFile file);
    }
}