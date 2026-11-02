using UniversityManagementSystem.Domain.Common;
using System;

namespace UniversityManagementSystem.Domain.Entities;

public class AcademicProgram : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
}
