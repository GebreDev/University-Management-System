using System;
using UniversityManagementSystem.Domain.Entities;

namespace UniversityManagementSystem.Application.DTOs.Registration;

public class SemesterOfferedCourseListDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int SemesterId { get; set; }
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public bool IsStudentRegistered { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static SemesterOfferedCourseListDto FromEntity(SemesterOfferedCourse src)
    {
        return new SemesterOfferedCourseListDto
        {
            Id = src.Id,
            SemesterId = src.SemesterId,
            CourseId = src.CourseId,
            CourseCode = src.Course?.Code ?? string.Empty,
            CourseTitle = src.Course?.Title ?? string.Empty,
            Credits = src.Course?.Credits ?? 0,
            DepartmentId = src.DepartmentId,
            DepartmentName = src.Department?.Name ?? string.Empty,
            Capacity = src.Capacity,
            RegisteredCount = 0,
            IsStudentRegistered = false,
            CreatedAt = src.CreatedAt,
            UpdatedAt = src.UpdatedAt
        };
    }
}
