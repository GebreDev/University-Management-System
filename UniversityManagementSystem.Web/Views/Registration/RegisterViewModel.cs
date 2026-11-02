using System.Collections.Generic;
using UniversityManagementSystem.Application.DTOs.Registration;

namespace UniversityManagementSystem.Web.Views.Registration
{
    public class RegisterViewModel
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public List<SemesterOfferedCourseListDto> Offerings { get; set; } = new();
        public IEnumerable<RegistrationListDto> ExistingRegistrations { get; set; } = new List<RegistrationListDto>();
    }
}
