using System.Collections.Generic;
using UniversityManagementSystem.Application.DTOs.Registration;

namespace UniversityManagementSystem.Web.Views.Registration
{
    public class AdminSemesterRegistrationsViewModel
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public List<RegistrationListDto> Registrations { get; set; } = new();
        public List<RegistrationListDto> PendingRegistrations { get; set; } = new();
    }
}
