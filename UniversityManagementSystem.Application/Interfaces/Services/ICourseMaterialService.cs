using UniversityManagementSystem.Application.DTOs.CourseMaterial;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface ICourseMaterialService
    {
        Task<CourseMaterialViewDto> UploadAsync(CourseMaterialUploadDto dto, string uploaderUserId);
        Task<CourseMaterialViewDto> UpdateAsync(CourseMaterialUpdateDto dto, string uploaderUserId);
        Task DeleteAsync(int id, string requesterUserId, bool isAdmin);

        /// <summary>Access: admins, the uploading instructor, or students enrolled in the course.</summary>
        Task<CourseMaterialViewDto> GetByIdAsync(int id, string requesterUserId, bool isAdmin, int? studentId = null);

        Task<IEnumerable<CourseMaterialListDto>> GetForInstructorAsync(string instructorUserId);
        Task<IEnumerable<CourseMaterialListDto>> GetForStudentAsync(int studentId);
        Task IncrementDownloadCountAsync(int id);
        Task<bool> CanManageAsync(int materialId, string userId, bool isAdmin);
        Task<bool> StudentHasAccessAsync(int materialId, int studentId);
    }
}
