using UniversityManagementSystem.Application.DTOs.Finance;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    /// <summary>
    /// Student-facing finance portal: balances, invoices, online payment (Chapa)
    /// and printable receipts. Students can only ever see and pay their own data.
    /// </summary>
    [Authorize(Roles = "Student")]
    public class StudentFinanceController : Controller
    {
        private readonly IFeeService _feeService;
        private readonly IPaymentService _paymentService;
        private readonly ApplicationDbContext _context;

        public StudentFinanceController(
            IFeeService feeService,
            IPaymentService paymentService,
            ApplicationDbContext context)
        {
            _feeService = feeService;
            _paymentService = paymentService;
            _context = context;
        }

        private async Task<Domain.Entities.Student?> GetCurrentStudentAsync()
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var user = await _context.Users.Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);
            return user?.Student;
        }

        public async Task<IActionResult> MyFinance()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return RedirectToAction("Index", "Home");

            // Run the clearance check FIRST: for a self-sponsored student with
            // no invoice yet it auto-assesses one, so the invoice (and its Pay
            // Now button) exists on the very first visit.
            var settings = await FeeSettingsHelper.GetAsync(_context);
            var latestSemester = await _context.Semesters
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();
            if (latestSemester != null)
            {
                try
                {
                    var clearance = await _feeService.CheckRegistrationClearanceAsync(student.Id, latestSemester.Id);
                    ViewBag.Clearance = clearance;
                }
                catch (InvalidOperationException ex)
                {
                    // Never let a clearance-check failure crash the finance page.
                    // Show it as a banner instead (the service normally returns a
                    // friendly result; this guards other config states too).
                    ViewBag.Clearance = new FinanceClearanceResult
                    {
                        IsCleared = false,
                        Message = ex.Message,
                        Category = student.PaymentCategory
                    };
                }
            }
            ViewBag.GateEnabled = settings.RegistrationGateEnabled;

            var model = await _feeService.GetStudentFinanceAsync(student.Id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayNow(int invoiceId)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return RedirectToAction("Index", "Home");

            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
                var checkoutUrl = await _paymentService.InitializeChapaPaymentAsync(student.Id, invoiceId, baseUrl);
                return Redirect(checkoutUrl);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(MyFinance));
            }
        }

        /// <summary>
        /// Printable receipt. Ownership is enforced: a student can never open
        /// another student's receipt by guessing the invoice id.
        /// </summary>
        public async Task<IActionResult> Receipt(int invoiceId)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return RedirectToAction("Index", "Home");

            var invoice = await _feeService.GetInvoiceAsync(invoiceId);
            if (invoice == null || invoice.StudentId != student.Id) return NotFound();

            return View(invoice);
        }
    }
}
