using UniversityManagementSystem.Application.DTOs.Attendance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IAttendanceService
    {
        Task MarkAttendanceAsync(AttendanceCreateDto dto);
        Task MarkBulkAttendanceAsync(BulkAttendanceDto dto);
        Task UpdateAttendanceAsync(AttendanceUpdateDto dto);
        Task<IEnumerable<AttendanceListDto>> GetAttendanceByStudentIdAsync(int studentId);
        Task<IEnumerable<AttendanceListDto>> GetAttendanceByCourseIdAsync(int courseId);
        Task<AttendanceSummaryDto> GetAttendanceSummaryAsync(int studentId, int? courseId = null);
        Task<AttendanceStatsDto> GetAttendanceStatsAsync();
        Task<IEnumerable<AttendanceReportDto>> GetAllAttendanceReportsAsync();
        Task<AttendanceListDto> GetAttendanceByIdAsync(int id);
    }
}