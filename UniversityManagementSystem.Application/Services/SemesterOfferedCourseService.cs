using UniversityManagementSystem.Application.DTOs.Registration;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;

namespace UniversityManagementSystem.Application.Services;

public class SemesterOfferedCourseService : ISemesterOfferedCourseService
{
    private readonly ISemesterOfferedCourseRepository _repository;

    public SemesterOfferedCourseService(ISemesterOfferedCourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SemesterOfferedCourseListDto>> GetOpenBySemesterIdAsync(int semesterId)
    {
        var entities = await _repository.GetOpenBySemesterIdAsync(semesterId).ConfigureAwait(false);
        return entities.Select(SemesterOfferedCourseListDto.FromEntity);
    }

    public async Task<IEnumerable<SemesterOfferedCourseListDto>> GetAllBySemesterIdAsync(int semesterId)
    {
        var entities = await _repository.GetBySemesterIdAsync(semesterId).ConfigureAwait(false);
        return entities.Select(SemesterOfferedCourseListDto.FromEntity);
    }

    public async Task<SemesterOfferedCourseListDto?> GetByIdWithDetailsAsync(int id)
    {
        var entity = await _repository.GetByIdWithDetailsAsync(id).ConfigureAwait(false);
        return entity == null ? null : SemesterOfferedCourseListDto.FromEntity(entity);
    }

    public async Task<int> GetRegisteredCountAsync(int offeredCourseId)
        => await _repository.GetRegisteredCountAsync(offeredCourseId).ConfigureAwait(false);

    public async Task<bool> IsStudentRegisteredAsync(int studentId, int offeredCourseId)
        => await _repository.IsStudentRegisteredAsync(studentId, offeredCourseId).ConfigureAwait(false);

    public async Task<IEnumerable<OfferedCourseScheduleDto>> GetSchedulesAsync(int offeredCourseId)
    {
        var schedules = await _repository.GetSchedulesAsync(offeredCourseId).ConfigureAwait(false);
        return schedules.Select(s => new OfferedCourseScheduleDto
        {
            Id = s.Id,
            DayOfWeek = s.DayOfWeek,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Room = s.Room,
            SemesterOfferedCourseId = s.SemesterOfferedCourseId
        });
    }

    public async Task<IEnumerable<SemesterOfferedCourseListDto>> GetAvailableForStudentAsync(
        int studentId,
        int semesterId,
        int academicProgramId)
    {
        var offerings = await _repository.GetOpenBySemesterIdAsync(semesterId).ConfigureAwait(false);
        return offerings.Where(o => o.DepartmentId == academicProgramId)
            .Select(SemesterOfferedCourseListDto.FromEntity);
    }
}
