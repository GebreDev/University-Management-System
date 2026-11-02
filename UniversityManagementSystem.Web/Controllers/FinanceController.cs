using UniversityManagementSystem.Application.DTOs.Finance;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using UniversityManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    /// <summary>
    /// Admin-controlled finance management: fee structures, invoices, payment
    /// verification, student payment categories and finance settings.
    /// Every mutation is audit-logged.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class FinanceController : Controller
    {
        private readonly IFeeService _feeService;
        private readonly IPaymentService _paymentService;
        private readonly ApplicationDbContext _context;

        public FinanceController(
            IFeeService feeService,
            IPaymentService paymentService,
            ApplicationDbContext context)
        {
            _feeService = feeService;
            _paymentService = paymentService;
            _context = context;
        }

        private string UserId
        {
            get
            {
                var name = User.Identity?.Name ?? "";
                return _context.Users.FirstOrDefault(u => u.UserName == name || u.Email == name)?.Id ?? "";
            }
        }

        // ================= Dashboard =================

        public async Task<IActionResult> Index(int? semesterId)
        {
            ViewBag.Semesters = new SelectList(await _context.Semesters.Include(s => s.AcademicYear).OrderByDescending(s => s.StartDate).ToListAsync(),
                "Id", "Name", semesterId);
            ViewBag.SelectedSemesterId = semesterId;
            var model = await _feeService.GetDashboardAsync(semesterId);
            return View(model);
        }

        // ================= Reports (analytics) =================

        /// <summary>Collection trends per semester and per department.</summary>
        public async Task<IActionResult> Reports(int? academicYearId)
        {
            ViewBag.AcademicYears = new SelectList(
                await _context.AcademicYears.OrderBy(y => y.Name).ToListAsync(), "Id", "Name", academicYearId);
            ViewBag.SelectedYearId = academicYearId;
            var model = await _feeService.GetFinanceAnalyticsAsync(academicYearId);
            return View(model);
        }

        // ================= Fee structures =================

        public async Task<IActionResult> FeeStructures(int? academicYearId, int? departmentId)
        {
            ViewBag.AcademicYears = new SelectList(await _context.AcademicYears.OrderBy(y => y.Name).ToListAsync(), "Id", "Name", academicYearId);
            ViewBag.Departments = new SelectList(await _context.Departments.OrderBy(d => d.Name).ToListAsync(), "Id", "Name", departmentId);
            ViewBag.SelectedYearId = academicYearId;
            ViewBag.SelectedDepartmentId = departmentId;
            var model = await _feeService.GetFeeStructuresAsync(academicYearId, departmentId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFeeStructure(FeeStructureCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                await _feeService.CreateFeeStructureAsync(dto, UserId);
                TempData["Success"] = "Fee structure created.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(FeeStructures), new { academicYearId = dto.AcademicYearId, departmentId = dto.DepartmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFeeStructure(int id, FeeStructureCreateDto dto)
        {
            try
            {
                await _feeService.UpdateFeeStructureAsync(id, dto, UserId);
                TempData["Success"] = "Fee structure updated. Existing invoices are not changed.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(FeeStructures), new { academicYearId = dto.AcademicYearId, departmentId = dto.DepartmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFeeStructure(int id)
        {
            try
            {
                await _feeService.DeleteFeeStructureAsync(id, UserId);
                TempData["Success"] = "Fee structure removed (or deactivated if used by invoices).";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(FeeStructures));
        }

        // ================= Invoices =================

        public async Task<IActionResult> Invoices(int? semesterId, InvoiceStatus? status, string? search)
        {
            ViewBag.Semesters = new SelectList(await _context.Semesters.Include(s => s.AcademicYear).OrderByDescending(s => s.StartDate).ToListAsync(), "Id", "Name", semesterId);
            ViewBag.SelectedSemesterId = semesterId;
            ViewBag.Status = status;
            ViewBag.Search = search;

            var model = await GetInvoicesFilteredAsync(semesterId, status, search);
            return View(model);
        }

        private async Task<System.Collections.Generic.List<InvoiceDto>> GetInvoicesFilteredAsync(int? semesterId, InvoiceStatus? status, string? search)
        {
            var query = _context.Invoices
                .Include(i => i.Student)
                .Include(i => i.Semester)
                .Include(i => i.Payments)
                .Include(i => i.Waivers)
                .AsQueryable();

            if (semesterId.HasValue) query = query.Where(i => i.SemesterId == semesterId.Value);
            if (status.HasValue) query = query.Where(i => i.Status == status.Value);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(i => i.InvoiceNumber.Contains(term)
                    || (i.Student!.FirstName + " " + i.Student.LastName).Contains(term)
                    || i.Student!.StudentId.Contains(term));
            }

            var list = await query.OrderByDescending(i => i.CreatedAt).Take(300).ToListAsync();
            return list.Select(i =>
            {
                var paid = i.Payments.Where(p => p.Status == PaymentStatus.Verified).Sum(p => p.Amount);
                var waived = i.Waivers.Sum(w => w.Amount);
                return new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    StudentId = i.StudentId,
                    StudentName = i.Student != null ? i.Student.FirstName + " " + i.Student.LastName : "",
                    StudentNumber = i.Student?.StudentId ?? "",
                    PaymentCategory = i.PaymentCategory,
                    SemesterName = i.Semester?.Name ?? "",
                    TotalAmount = i.TotalAmount,
                    PaidAmount = paid,
                    WaivedAmount = waived,
                    Balance = i.Status == InvoiceStatus.Cancelled ? 0 : i.TotalAmount - paid - waived,
                    DueDate = i.DueDate,
                    Status = i.Status,
                    CreatedAt = i.CreatedAt
                };
            }).ToList();
        }

        public async Task<IActionResult> InvoiceDetails(int id)
        {
            var invoice = await _feeService.GetInvoiceAsync(id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantWaiver(int invoiceId, decimal amount, string reason)
        {
            try
            {
                await _feeService.GrantWaiverAsync(invoiceId, amount, reason, UserId);
                TempData["Success"] = "Waiver granted.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(InvoiceDetails), new { id = invoiceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelInvoice(int invoiceId)
        {
            try
            {
                await _feeService.CancelInvoiceAsync(invoiceId, UserId);
                TempData["Success"] = "Invoice cancelled.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Invoices));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssessInvoice(int studentId, int semesterId)
        {
            try
            {
                await _feeService.AssessSemesterAsync(studentId, semesterId);
                TempData["Success"] = "Invoice assessed for the student.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Invoices));
        }

        // ================= Payments =================

        public async Task<IActionResult> Payments(int? semesterId, PaymentStatus? status)
        {
            ViewBag.Semesters = new SelectList(await _context.Semesters.Include(s => s.AcademicYear).OrderByDescending(s => s.StartDate).ToListAsync(), "Id", "Name", semesterId);
            ViewBag.SelectedSemesterId = semesterId;
            ViewBag.Status = status;
            var model = await _paymentService.GetPaymentsAsync(semesterId, status);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordManualPayment(ManualPaymentCreateDto dto)
        {
            try
            {
                await _paymentService.RecordManualPaymentAsync(dto, UserId);
                TempData["Success"] = "Manual payment recorded. It now needs verification.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Payments));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPayment(int paymentId)
        {
            try
            {
                await _paymentService.VerifyManualPaymentAsync(paymentId, UserId);
                TempData["Success"] = "Payment verified and posted to the ledger.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Payments));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPayment(int paymentId, string reason)
        {
            try
            {
                await _paymentService.RejectManualPaymentAsync(paymentId, UserId, reason);
                TempData["Success"] = "Payment rejected.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Payments));
        }

        /// <summary>
        /// Refunds a verified payment. The ledger stays append-only: a reversal
        /// charge is posted and the payment is marked Refunded, never deleted.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefundPayment(int paymentId, string reason)
        {
            try
            {
                await _paymentService.RefundPaymentAsync(paymentId, reason, UserId);
                TempData["Success"] = "Payment refunded and the student's balance restored.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Payments));
        }

        // ================= Exports =================

        public async Task<IActionResult> ExportInvoices(int? semesterId, InvoiceStatus? status, string format = "excel")
        {
            try
            {
                var bytes = await _paymentService.ExportInvoicesAsync(semesterId, status, format);
                var isExcel = string.Equals(format, "excel", StringComparison.OrdinalIgnoreCase);
                var ext = isExcel ? "xlsx" : "pdf";
                var contentType = isExcel
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "application/pdf";
                return File(bytes, contentType, $"Invoices_{DateTime.Now:yyyyMMdd}.{ext}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Export failed: {ex.Message}";
                return RedirectToAction(nameof(Invoices), new { semesterId, status });
            }
        }

        public async Task<IActionResult> ExportPayments(int? semesterId, PaymentStatus? status, string format = "excel")
        {
            try
            {
                var bytes = await _paymentService.ExportPaymentsAsync(semesterId, status, format);
                var isExcel = string.Equals(format, "excel", StringComparison.OrdinalIgnoreCase);
                var ext = isExcel ? "xlsx" : "pdf";
                var contentType = isExcel
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "application/pdf";
                return File(bytes, contentType, $"Payments_{DateTime.Now:yyyyMMdd}.{ext}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Export failed: {ex.Message}";
                return RedirectToAction(nameof(Payments), new { semesterId, status });
            }
        }

        // ================= Student payment categories =================

        public async Task<IActionResult> Students(string? search)
        {
            var query = _context.Students.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(s => (s.FirstName + " " + s.LastName).Contains(term) || s.StudentId.Contains(term));
            }

            var students = await query.OrderBy(s => s.FirstName).Take(200)
                .Select(s => new
                {
                    s.Id,
                    s.FirstName,
                    s.LastName,
                    s.StudentId,
                    s.PaymentCategory
                }).ToListAsync();

            ViewBag.Search = search;
            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPaymentCategory(int studentId, PaymentCategory category)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) return NotFound();

            var old = student.PaymentCategory;
            student.PaymentCategory = category;
            student.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _context.AuditLogs.Add(new Domain.Entities.AuditLog
            {
                UserId = UserId,
                UserName = User.Identity?.Name ?? "",
                UserRole = "Admin",
                Action = "Update",
                Entity = "StudentPaymentCategory",
                EntityId = student.Id,
                Details = $"Payment category changed from {old} to {category} for {student.FirstName} {student.LastName} ({student.StudentId}).",
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Payment category updated for {student.FirstName} {student.LastName}.";
            return RedirectToAction(nameof(Students));
        }

        // ================= Settings =================

        public async Task<IActionResult> Settings()
        {
            var settings = await FeeSettingsHelper.GetAsync(_context);
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(FinanceSetting model)
        {
            var settings = await FeeSettingsHelper.GetAsync(_context);
            settings.RegistrationGateEnabled = model.RegistrationGateEnabled;
            settings.FirstInstallmentPercent = Math.Clamp(model.FirstInstallmentPercent, 0m, 100m);
            settings.ChapaBaseUrl = model.ChapaBaseUrl?.Trim() ?? string.Empty;
            settings.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Finance settings saved.";
            return RedirectToAction(nameof(Settings));
        }
    }
}
