using Microsoft.AspNetCore.Http;

namespace UniversityManagementSystem.Application.DTOs.UserManagement
{
    public class ProfilePictureDto
    {
        public IFormFile ImageFile { get; set; }
    }
}