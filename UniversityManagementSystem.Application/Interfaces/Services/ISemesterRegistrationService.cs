using UniversityManagementSystem.Application.DTOs.Registration;
using UniversityManagementSystem.Domain.Enums;
using System.Linq.Expressions;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface ISemesterRegistrationService
{
    Task<SemesterRegistrationDto?> GetByIdAsync(int id);
    Task<IEnumerable<SemesterRegistrationDto>> GetAllAsync();
    Task<IQueryable<SemesterRegistrationDto>> FindAsync(Expression<Func<SemesterRegistrationDto, bool>> predicate);
    Task<IEnumerable<SemesterRegistrationDto>> GetByStudentIdAsync(int studentId);
    Task<SemesterRegistrationDto?> GetActiveForStudentAsync(int studentId, int semesterId);
    Task<SemesterRegistrationDto> CreateForStudentAsync(
        int studentId,
        int academicProgramId,
        int semesterId,
        Dictionary<string, string> notes);
    Task<SemesterRegistrationDto> UpdateStatusAsync(
        int id,
        RegistrationStatus status,
        string? remarks);
}

