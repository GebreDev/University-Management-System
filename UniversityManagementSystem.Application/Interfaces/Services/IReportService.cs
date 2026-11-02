using UniversityManagementSystem.Application.DTOs.Reports;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<byte[]> GenerateReportAsync(string reportType, string format, ExportFilterDto? filter = null);
    }
}