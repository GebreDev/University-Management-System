using UniversityManagementSystem.Application.DTOs.Registration;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using System.Linq.Expressions;

namespace UniversityManagementSystem.Application.Services;

public partial class SemesterRegistrationService : ISemesterRegistrationService
{
    private readonly ISemesterRegistrationRepository _repository;
    public SemesterRegistrationService(
        ISemesterRegistrationRepository repository)
    {
        _repository = repository;
    }

    public async Task<SemesterRegistrationDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return null;

        return MapToDto(entity);
    }

    public async Task<IQueryable<SemesterRegistrationDto>> FindAsync(Expression<Func<SemesterRegistrationDto, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SemesterRegistrationDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<IEnumerable<SemesterRegistrationDto>> GetByStudentIdAsync(int studentId)
    {
        var entities = await _repository.GetByStudentIdAsync(studentId);
        return entities.Select(MapToDto);
    }

    public async Task<SemesterRegistrationDto?> GetActiveForStudentAsync(int studentId, int semesterId)
    {
        var entity = await _repository.GetByStudentAndSemesterAsync(studentId, semesterId);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<SemesterRegistrationDto> CreateForStudentAsync(
        int studentId,
        int academicProgramId,
        int semesterId,
        Dictionary<string, string> notes)
    {
        var existing = await _repository.GetByStudentAndSemesterAsync(studentId, semesterId);
        if (existing != null)
            throw new InvalidOperationException("Student already has a semester registration for this semester.");

        var entity = new SemesterRegistration
        {
            StudentId = studentId,
            AcademicProgramId = academicProgramId,
            SemesterId = semesterId,
            Status = RegistrationStatus.Draft,
            Notes = notes.GetValueOrDefault("Notes"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            RegisteredAt = null
        };

        var added = await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return MapToDto(added);
    }

    public async Task<SemesterRegistrationDto> UpdateStatusAsync(
        int id,
        RegistrationStatus status,
        string? remarks)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new InvalidOperationException("Semester registration not found.");

        entity.Status = status;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.Notes = remarks != null ? remarks : entity.Notes;

        var updated = await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();

        return MapToDto(updated);
    }

    private static SemesterRegistrationDto MapToDto(SemesterRegistration entity)
    {
        var student = entity.Student;
        var program = entity.AcademicProgram;
        var semester = entity.Semester;

        return new SemesterRegistrationDto
        {
            Id = entity.Id,
            StudentId = entity.StudentId,
            AcademicProgramId = entity.AcademicProgramId,
            SemesterId = entity.SemesterId!.Value,
            Status = entity.Status,
            Notes = entity.Notes,
            RegisteredAt = entity.RegisteredAt,
            EarnedCredits = entity.EarnedCredits,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Student = student != null && student.Student != null
                ? new SemesterRegistrationStudentDto
                {
                    Id = student.Student.Id,
                    FirstName = student.Student.FirstName,
                    LastName = student.Student.LastName,
                    StudentNumber = student.Student.StudentId
                }
                : null!,
            AcademicProgram = program != null
                ? new SemesterRegistrationProgramDto
                {
                    Id = program.Id,
                    Name = program.Name,
                    Code = program.Code,
                    DepartmentId = program.DepartmentId,
                    DepartmentName = program.Department != null ? program.Department.Name : string.Empty
                }
                : null!,
            Semester = semester != null
                ? new SemesterRegistrationDto.SemesterDto
                {
                    Id = semester.Id,
                    Name = semester.Name,
                    AcademicYearName = semester.AcademicYear != null
                        ? semester.AcademicYear.Name
                        : string.Empty,
                    StartDate = semester.StartDate,
                    EndDate = semester.EndDate
                }
                : null!
        };
    }
}