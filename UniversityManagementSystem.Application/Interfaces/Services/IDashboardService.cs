using UniversityManagementSystem.Application.DTOs.Dashboard;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync(string adminName);
        Task<InstructorDashboardDto> GetInstructorDashboardAsync(int instructorId, string instructorName);
        Task<StudentDashboardDto> GetStudentDashboardAsync(int studentId, string studentName);
    }
}
