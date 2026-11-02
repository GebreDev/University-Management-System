using UniversityManagementSystem.Application.DTOs.Student;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;

namespace UniversityManagementSystem.Application.DTOs.Registration;

public partial class SemesterRegistrationDto
{
    public int Id { get; set; }

    public SemesterRegistrationStudentDto? Student { get; set; }

    public int StudentId { get; set; }

    public SemesterRegistrationProgramDto? AcademicProgram { get; set; }

    public int AcademicProgramId { get; set; }

    public SemesterRegistrationDto.SemesterDto? Semester { get; set; }

    public int? SemesterId { get; set; }

    public int EarnedCredits { get; set; }

    public DateTime? RegisteredAt { get; set; }

    public RegistrationStatus Status { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public partial class SemesterRegistrationDto
{
    public partial class SemesterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AcademicYearName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

public class SemesterRegistrationStudentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
}

public class SemesterRegistrationProgramDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}

