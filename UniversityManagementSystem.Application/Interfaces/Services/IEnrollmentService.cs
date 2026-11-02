using UniversityManagementSystem.Application.DTOs.Enrollment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<EnrollmentListDto>> GetAllEnrollmentsAsync();
        Task<EnrollmentViewDto> GetEnrollmentByIdAsync(int id);
        Task<EnrollmentViewDto> CreateEnrollmentAsync(EnrollmentCreateDto dto);
        Task UpdateEnrollmentAsync(EnrollmentUpdateDto dto);
        Task DeleteEnrollmentAsync(int id);
        Task ApproveEnrollmentAsync(int id);
        Task RejectEnrollmentAsync(int id, string reason);
        Task<IEnumerable<EnrollmentListDto>> GetEnrollmentsByStudentAsync(int studentId);
        Task<IEnumerable<EnrollmentListDto>> GetEnrollmentsByCourseAsync(int courseId);
        Task<IEnumerable<EnrollmentListDto>> GetPendingEnrollmentsAsync();
        Task<bool> IsStudentAlreadyEnrolledAsync(int studentId, int courseId);
    }
}