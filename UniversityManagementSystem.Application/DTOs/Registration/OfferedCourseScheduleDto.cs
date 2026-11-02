using UniversityManagementSystem.Domain.Entities;
using System;

namespace UniversityManagementSystem.Application.DTOs.Registration;

public class OfferedCourseScheduleDto
{
    public int Id { get; set; }
    public int SemesterOfferedCourseId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Room { get; set; } = string.Empty;
}
