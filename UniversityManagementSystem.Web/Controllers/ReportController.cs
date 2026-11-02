using UniversityManagementSystem.Application.DTOs.Reports;
using UniversityManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export(ExportFilterDto filter, string format)
        {
            try
            {
                filter ??= new ExportFilterDto();
                var fileBytes = await _reportService.GenerateReportAsync(filter.ReportType, format, filter);
                var isExcel = string.Equals(format, "excel", StringComparison.OrdinalIgnoreCase);
                var ext = isExcel ? "xlsx" : "pdf";
                var contentType = isExcel
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "application/pdf";
                return File(fileBytes, contentType, $"Report_{filter.ReportType}_{DateTime.Now:yyyyMMdd}.{ext}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to export report: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}