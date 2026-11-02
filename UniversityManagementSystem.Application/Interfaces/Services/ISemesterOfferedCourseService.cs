using UniversityManagementSystem.Application.DTOs.Registration;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface ISemesterOfferedCourseService
{
    Task<IEnumerable<SemesterOfferedCourseListDto>> GetOpenBySemesterIdAsync(int semesterId);
    Task<IEnumerable<SemesterOfferedCourseListDto>> GetAllBySemesterIdAsync(int semesterId);
    Task<SemesterOfferedCourseListDto?> GetByIdWithDetailsAsync(int id);
    Task<int> GetRegisteredCountAsync(int offeredCourseId);
    Task<bool> IsStudentRegisteredAsync(int studentId, int offeredCourseId);
    Task<IEnumerable<OfferedCourseScheduleDto>> GetSchedulesAsync(int offeredCourseId);
    Task<IEnumerable<SemesterOfferedCourseListDto>> GetAvailableForStudentAsync(
        int studentId,
        int semesterId,
        int academicProgramId);
}
