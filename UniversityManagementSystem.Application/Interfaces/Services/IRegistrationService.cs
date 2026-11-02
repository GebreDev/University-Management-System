using UniversityManagementSystem.Application.DTOs.Registration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IRegistrationService
    {
        Task<IEnumerable<SemesterOfferedCourseListDto>> GetOpenOfferingsBySemesterAsync(int semesterId, int studentId);
        Task<RegistrationSubmissionResultDto> SubmitRegistrationAsync(int studentId, int semesterId, IEnumerable<int> offeredCourseIds);
        Task<RegistrationDetailDto?> GetRegistrationAsync(int id, int studentId);
        Task<RegistrationDetailDto?> GetRegistrationForReviewAsync(int id);
        Task<IEnumerable<RegistrationListDto>> GetStudentRegistrationsAsync(int studentId);
        Task<IEnumerable<RegistrationListDto>> GetSemesterRegistrationsAsync(int semesterId);
        Task<IEnumerable<RegistrationListDto>> GetPendingRegistrationsAsync(int semesterId);
        Task<RegistrationListDto> ReviewRegistrationAsync(int id, RegistrationReviewDto dto);
        Task<RegistrationListDto> WithdrawRegistrationAsync(int id, int studentId);
    }
}
