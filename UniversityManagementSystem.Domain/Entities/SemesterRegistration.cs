using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities;

public class SemesterRegistration : BaseEntity
{
    public int StudentId { get; set; }
    public ApplicationUser? Student { get; set; }

    public int AcademicProgramId { get; set; }
    public AcademicProgram? AcademicProgram { get; set; }

    public int? SemesterId { get; set; }
    public Semester? Semester { get; set; }

    public int EarnedCredits { get; set; }
    public DateTime? RegisteredAt { get; set; }

    public new RegistrationStatus Status { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<SemesterRegistrationCourse> SemesterRegistrationCourses { get; set; } = new List<SemesterRegistrationCourse>();
}

public class SemesterRegistrationCourse : BaseEntity
{
    public int SemesterRegistrationId { get; set; }
    public SemesterRegistration? SemesterRegistration { get; set; }

    public int SemesterOfferedCourseId { get; set; }
    public SemesterOfferedCourse? SemesterOfferedCourse { get; set; }

    public new RegistrationItemStatus Status { get; set; }

    public DateTime? RegisteredAt { get; set; }
    public string? Remarks { get; set; }
}


