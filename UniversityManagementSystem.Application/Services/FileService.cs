using UniversityManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class FileService : IFileService
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public FileService()
        {
            var dirs = new[] { "profiles", "assignments", "submissions", "chat", "exams", "course-materials" };
            foreach (var dir in dirs)
            {
                var fullPath = Path.Combine(_basePath, dir);
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
            }
        }

        public async Task<string?> SaveFileAsync(string base64Content, string folder, string? fileName = null)
        {
            if (string.IsNullOrEmpty(base64Content))
                return null;

            var ext = "bin";
            if (!string.IsNullOrEmpty(fileName))
                ext = Path.GetExtension(fileName)?.TrimStart('.') ?? "bin";

            if (base64Content.Contains(","))
                base64Content = base64Content.Substring(base64Content.IndexOf(",") + 1);

            var bytes = Convert.FromBase64String(base64Content);
            var fileNameGuid = $"{Guid.NewGuid()}.{ext}";
            var folderPath = Path.Combine(_basePath, folder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileNameGuid);
            await File.WriteAllBytesAsync(filePath, bytes);

            if (folder == "profiles" && (ext == "jpg" || ext == "jpeg" || ext == "png" || ext == "gif"))
            {
                await ResizeProfileImageAsync(filePath);
            }

            return $"/uploads/{folder}/{fileNameGuid}";
        }

        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return;
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public async Task<byte[]?> GetFileAsync(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));
            if (!File.Exists(fullPath))
                return null;
            return await File.ReadAllBytesAsync(fullPath);
        }

        private static readonly long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx", ".xlsx", ".csv", ".txt", ".zip"
        };

        // Material extensions for the Course Materials feature: PDF, DOC/DOCX,
        // PPT/PPTX, XLS/XLSX, TXT, ZIP and common images.
        private static readonly HashSet<string> MaterialExtensions = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx",
            ".txt", ".zip", ".jpg", ".jpeg", ".png", ".gif"
        };

        public async Task<string?> SaveMaterialFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            if (file.Length > MaxFileSize)
                throw new InvalidOperationException($"File size exceeds the 10MB limit. Your file is {(file.Length / 1024.0 / 1024.0):F2}MB.");

            var ext = Path.GetExtension(file.FileName);
            if (!MaterialExtensions.Contains(ext))
                throw new InvalidOperationException($"File type '{ext}' is not allowed. Allowed types: PDF, DOC, DOCX, PPT, PPTX, XLS, XLSX, TXT, ZIP, JPG, PNG, GIF.");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var folderPath = Path.Combine(_basePath, "course-materials");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/course-materials/{fileName}";
        }

        public async Task<string?> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            if (file.Length > MaxFileSize)
                throw new InvalidOperationException($"File size exceeds the 10MB limit. Your file is {(file.Length / 1024.0 / 1024.0):F2}MB.");

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(ext))
                throw new InvalidOperationException($"File extension '{ext}' is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var folderPath = Path.Combine(_basePath, folder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<string?> SaveProfilePictureAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                return null;
            if (file.Length > MaxFileSize)
                throw new InvalidOperationException($"File size exceeds the 10MB limit. Your file is {(file.Length / 1024.0 / 1024.0):F2}MB.");
            var ext = Path.GetExtension(file.FileName);
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            if (!allowedImageExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Please select a JPG, PNG, or GIF image.");
            var fileName = $"{userId}{ext}";
            var folderPath = Path.Combine(_basePath, "profiles");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, fileName);
            // Use a scoped using block to ensure the file stream is closed
            // BEFORE ResizeProfileImageAsync tries to overwrite the file.
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // Stream is now closed; safe to read + overwrite via ImageSharp.
            await ResizeProfileImageAsync(filePath);
            return $"/uploads/profiles/{fileName}";
        }

        private static async Task ResizeProfileImageAsync(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Invalid profile image path.");
            var temporaryPath = Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(filePath)}-{Guid.NewGuid()}{Path.GetExtension(filePath)}");
            try
            {
                using (var input = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var image = await Image.LoadAsync(input))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Crop,
                        Size = new Size(200, 200)
                    }));
                    await image.SaveAsync(temporaryPath);
                }

                // Atomic overwrite: safe because the outer SaveProfilePictureAsync
                // already closed its stream via the scoped using block.
                File.Move(temporaryPath, filePath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }
    }
}
