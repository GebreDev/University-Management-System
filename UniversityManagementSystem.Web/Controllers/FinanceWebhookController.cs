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
    /// Anonymous gateway endpoints. Security model: we never trust the payload -
    /// every callback is re-verified server-to-server with Chapa before a
    /// payment is marked verified. Webhook + return URL both funnel into the
    /// same idempotent verification path, so a payment can never be
    /// double-credited.
    /// </summary>
    [AllowAnonymous]
    public class FinanceWebhookController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ApplicationDbContext _context;

        public FinanceWebhookController(IPaymentService paymentService, ApplicationDbContext context)
        {
            _paymentService = paymentService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ChapaWebhook([FromForm] string? tx_ref, [FromForm] string? charge_id)
        {
            // Chapa posts form-encoded data; verification is done server-side.
            var ok = await _paymentService.HandleWebhookAsync(charge_id ?? "", tx_ref ?? "", Request.Headers["Chapa-Signature"].ToString());
            return Ok(new { received = ok });
        }

        [HttpGet]
        public async Task<IActionResult> ChapaReturn([FromQuery] string? tx_ref)
        {
            var verified = false;
            if (!string.IsNullOrWhiteSpace(tx_ref))
                verified = await _paymentService.VerifyByReferenceAsync(tx_ref);

            TempData[verified ? "Success" : "Error"] = verified
                ? "Payment confirmed successfully. Your balance has been updated."
                : "We could not confirm your payment yet. If you were charged, the finance office will verify it shortly.";

            return RedirectToAction(nameof(StudentFinanceController.MyFinance), "StudentFinance");
        }

        // ================= TEST MODE (no Chapa keys configured) =================

        /// <summary>
        /// Local stand-in for Chapa's hosted checkout page, shown only while no
        /// Chapa keys are configured. Lets you exercise the full payment order
        /// flow (Pay Now -> checkout -> confirm -> ledger -> receipt) without
        /// real money. Disappears automatically once CHAPA_SECRET_KEY is set.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MockCheckout(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference)) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Invoice)
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.Reference == reference);

            if (payment == null) return NotFound();

            // Ownership: only the owning student may see the mock checkout.
            var userName = User.Identity?.Name ?? "";
            var user = await _context.Users.Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);
            var isOwner = user?.Student != null && payment.StudentId == user.Student.Id;

            ViewBag.IsOwner = isOwner;
            return View(payment);
        }

        /// <summary>
        /// "Pay" button on the mock checkout: confirms the pending payment
        /// through the same verified pipeline as the real gateway.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MockPay(string reference)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Reference == reference);

            if (payment == null || !payment.Reference.StartsWith("MOCK-", StringComparison.Ordinal))
            {
                TempData["Error"] = "Test payment not found.";
                return RedirectToAction(nameof(StudentFinanceController.MyFinance), "StudentFinance");
            }

            var userName = User.Identity?.Name ?? "";
            var user = await _context.Users.Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);
            var student = user?.Student;
            if (student == null || payment.StudentId != student.Id)
            {
                TempData["Error"] = "You can only confirm your own test payments.";
                return RedirectToAction(nameof(StudentFinanceController.MyFinance), "StudentFinance");
            }

            var ok = await _paymentService.ConfirmMockPaymentAsync(payment.Id, student.Id);

            TempData[ok ? "Success" : "Error"] = ok
                ? $"Test payment of {payment.Amount:N2} ETB confirmed successfully. Your balance has been updated."
                : "This test payment could not be confirmed (it may already be processed).";

            return RedirectToAction(nameof(StudentFinanceController.MyFinance), "StudentFinance");
        }

        /// <summary>
        /// "Cancel" on the mock checkout: leaves the payment Pending (like an
        /// abandoned real checkout) and returns the student to My Finance.
        /// </summary>
        [HttpGet]
        public IActionResult MockCancel()
        {
            TempData["Info"] = "Test checkout cancelled - the payment was not completed.";
            return RedirectToAction(nameof(StudentFinanceController.MyFinance), "StudentFinance");
        }
    }
}
