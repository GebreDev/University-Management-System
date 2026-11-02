using UniversityManagementSystem.Application.DTOs.Finance;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    /// <summary>
    /// Payment handling: Chapa gateway checkout + webhook verification, and
    /// manual bank payments recorded/verified by finance officers.
    /// Nothing is trusted from the client: amounts come from our invoice and
    /// gateway confirmations are verified server-to-server before posting to
    /// the ledger. A verified payment can never be credited twice (idempotent).
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private const string ChapaInitializeUrl = "https://api.chapa.co/v1/transaction/initialize";
        private const string ChapaVerifyUrl = "https://api.chapa.co/v1/transaction/verify/";

        private readonly IFinanceRepository _financeRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuditLogService _auditLogService;
        private readonly INotificationService _notificationService;
        private readonly IEmailSenderService _emailSender;

        public PaymentService(
            IFinanceRepository financeRepo,
            IStudentRepository studentRepo,
            IHttpClientFactory httpClientFactory,
            IAuditLogService auditLogService,
            INotificationService notificationService,
            IEmailSenderService emailSender)
        {
            _financeRepo = financeRepo;
            _studentRepo = studentRepo;
            _httpClientFactory = httpClientFactory;
            _auditLogService = auditLogService;
            _notificationService = notificationService;
            _emailSender = emailSender;
        }

        // ================= Gateway (Chapa) =================

        /// <summary>Mock mode is active when no Chapa keys are configured.</summary>
        public bool IsMockMode => string.IsNullOrWhiteSpace(GetChapaSecretKey());

        public async Task<string> InitializeChapaPaymentAsync(int studentId, int invoiceId, string returnUrlBase)
        {
            var secretKey = GetChapaSecretKey();

            // ===== MOCK MODE (no Chapa keys configured) =====
            // Creates the same Pending payment record as the real flow so the
            // entire order pipeline (pending -> confirm -> ledger -> invoice
            // status -> notification) can be tested without real money.
            // Adding CHAPA_SECRET_KEY/CHAPA_PUBLIC_KEY later switches to the
            // real gateway automatically - no code changes needed.
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                var mockInvoice = await _financeRepo.GetInvoiceByIdAsync(invoiceId)
                    ?? throw new InvalidOperationException("Invoice not found.");
                if (mockInvoice.StudentId != studentId)
                    throw new InvalidOperationException("Invoice not found.");
                if (mockInvoice.Status == InvoiceStatus.Cancelled)
                    throw new InvalidOperationException("This invoice was cancelled. Please contact the registrar.");

                var mockApplied = mockInvoice.Payments.Where(p => p.Status == PaymentStatus.Verified).Sum(p => p.Amount)
                                  + mockInvoice.Waivers.Sum(w => w.Amount);
                var mockBalance = mockInvoice.TotalAmount - mockApplied;
                if (mockBalance <= 0)
                    throw new InvalidOperationException("This invoice is already fully settled.");

                var mockPayment = new Payment
                {
                    Reference = $"MOCK-{mockInvoice.Id}-{DateTime.UtcNow.Ticks}",
                    InvoiceId = mockInvoice.Id,
                    StudentId = studentId,
                    Amount = mockBalance,
                    Method = PaymentMethod.Chapa,
                    Status = PaymentStatus.Pending,
                    PayerName = mockInvoice.Student != null ? $"{mockInvoice.Student.FirstName} {mockInvoice.Student.LastName}" : null,
                    Remarks = "Test-mode payment (Chapa keys not configured)"
                };
                await _financeRepo.AddPaymentAsync(mockPayment);

                // Mirror the real gateway redirect via our local checkout page.
                return $"/FinanceWebhook/MockCheckout?reference={Uri.EscapeDataString(mockPayment.Reference)}";
            }

            // ===== REAL CHAPA MODE (keys configured) =====

            var invoice = await _financeRepo.GetInvoiceByIdAsync(invoiceId)
                ?? throw new InvalidOperationException("Invoice not found.");
            if (invoice.StudentId != studentId)
                throw new InvalidOperationException("Invoice not found.");

            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new InvalidOperationException("This invoice was cancelled. Please contact the registrar.");

            var applied = invoice.Payments.Where(p => p.Status == PaymentStatus.Verified).Sum(p => p.Amount)
                          + invoice.Waivers.Sum(w => w.Amount);
            var balance = invoice.TotalAmount - applied;
            if (balance <= 0)
                throw new InvalidOperationException("This invoice is already fully settled.");

            var reference = $"UMS-{invoice.Id}-{DateTime.UtcNow.Ticks}";

            var payment = new Payment
            {
                Reference = reference,
                InvoiceId = invoice.Id,
                StudentId = studentId,
                Amount = balance,
                Method = PaymentMethod.Chapa,
                Status = PaymentStatus.Pending,
                PayerName = invoice.Student != null ? $"{invoice.Student.FirstName} {invoice.Student.LastName}" : null
            };
            payment = await _financeRepo.AddPaymentAsync(payment);

            var payload = new
            {
                public_key = GetChapaPublicKey(),
                amount = balance.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                currency = "ETB",
                email = invoice.Student?.ApplicationUser?.Email ?? "finance@ums.edu.et",
                first_name = invoice.Student?.FirstName ?? "Student",
                last_name = invoice.Student?.LastName ?? "",
                tx_ref = reference,
                callback_url = $"{returnUrlBase.TrimEnd('/')}/FinanceWebhook/ChapaReturn",
                return_url = $"{returnUrlBase.TrimEnd('/')}/FinanceWebhook/ChapaReturn",
                customization = new
                {
                    title = "Tuition Payment",
                    description = $"Invoice {invoice.InvoiceNumber}"
                }
            };

            var client = _httpClientFactory.CreateClient("Chapa");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", secretKey);

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsJsonAsync(ChapaInitializeUrl, payload);
            }
            catch (HttpRequestException)
            {
                payment.Status = PaymentStatus.Failed;
                payment.Remarks = "Could not reach the payment gateway. Try again later.";
                await _financeRepo.UpdatePaymentAsync(payment);
                throw new InvalidOperationException("Could not reach the payment gateway. Please try again later.");
            }

            JsonElement json;
            try
            {
                json = await response.Content.ReadFromJsonAsync<JsonElement>();
            }
            catch (System.Text.Json.JsonException)
            {
                payment.Status = PaymentStatus.Failed;
                payment.Remarks = "Invalid response from the payment gateway.";
                await _financeRepo.UpdatePaymentAsync(payment);
                throw new InvalidOperationException("The payment gateway returned an invalid response. Please try again later.");
            }

            var status = json.TryGetProperty("status", out var st) && st.GetString() == "success";
            var checkoutUrl = json.TryGetProperty("data", out var data) && data.TryGetProperty("checkout_url", out var url)
                ? url.GetString() : null;

            if (!status || string.IsNullOrWhiteSpace(checkoutUrl))
            {
                payment.Status = PaymentStatus.Failed;
                payment.Remarks = "Gateway rejected the payment request.";
                await _financeRepo.UpdatePaymentAsync(payment);
                throw new InvalidOperationException("The payment gateway rejected the request. Please try again or pay at the bank.");
            }

            payment.CheckoutUrl = checkoutUrl;
            await _financeRepo.UpdatePaymentAsync(payment);
            return checkoutUrl;
        }

        public async Task<bool> VerifyByReferenceAsync(string txRef)
        {
            var payment = await _financeRepo.GetPaymentByReferenceAsync(txRef);
            if (payment == null) return false;
            if (payment.Status == PaymentStatus.Verified) return true; // idempotent

            // Mock payments never contact Chapa - they were confirmed locally
            // at MockCheckout. A MOCK reference in Chapa would be invalid anyway.
            if (payment.Reference.StartsWith("MOCK-", StringComparison.Ordinal))
                return payment.Status == PaymentStatus.Verified;

            var secretKey = GetChapaSecretKey();
            if (string.IsNullOrWhiteSpace(secretKey)) return false;

            var client = _httpClientFactory.CreateClient("Chapa");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", secretKey);

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync($"{ChapaVerifyUrl}{Uri.EscapeDataString(txRef)}");
            }
            catch (HttpRequestException)
            {
                return false;
            }
            if (!response.IsSuccessStatusCode) return false;

            JsonElement json;
            try
            {
                json = await response.Content.ReadFromJsonAsync<JsonElement>();
            }
            catch (System.Text.Json.JsonException)
            {
                return false;
            }

            var success = json.TryGetProperty("status", out var st) && st.GetString() == "success"
                          && json.TryGetProperty("data", out var data)
                          && data.TryGetProperty("status", out var dst)
                          && dst.GetString() == "success";

            if (!success) return false;

            await MarkVerifiedAsync(payment, json.TryGetProperty("data", out var d2) && d2.TryGetProperty("id", out var pid) ? pid.GetString() : null);
            return true;
        }

        public async Task<bool> HandleWebhookAsync(string chapaChargeId, string txRef, string signature)
        {
            // The webhook is best-effort; the return-URL verification is the
            // authoritative fallback when the webhook is missed or misconfigured.
            if (string.IsNullOrWhiteSpace(txRef)) return false;

            var payment = await _financeRepo.GetPaymentByReferenceAsync(txRef);
            if (payment == null) return false;
            if (payment.Status == PaymentStatus.Verified) return true;

            // Verify directly with Chapa rather than trusting the payload body.
            return await VerifyByReferenceAsync(txRef);
        }

        /// <summary>
        /// TEST MODE ONLY: confirms a pending mock payment locally (no Chapa
        /// call), running the exact same verified pipeline as the real gateway:
        /// ledger posting, invoice status update, notification. Ownership and
        /// pending-status guards make it safe even in test mode.
        /// </summary>
        public async Task<bool> ConfirmMockPaymentAsync(int paymentId, int studentId)
        {
            var payment = await _financeRepo.GetPaymentByIdAsync(paymentId);
            if (payment == null || payment.StudentId != studentId) return false;
            if (!payment.Reference.StartsWith("MOCK-", StringComparison.Ordinal)) return false; // real payments must go through Chapa
            if (payment.Status != PaymentStatus.Pending) return payment.Status == PaymentStatus.Verified;

            await MarkVerifiedAsync(payment, null, officerUserId: null);
            return true;
        }

        // ================= Manual (finance office) =================

        public async Task<PaymentDto> RecordManualPaymentAsync(ManualPaymentCreateDto dto, string officerUserId)
        {
            if (dto.Amount <= 0) throw new InvalidOperationException("Payment amount must be positive.");

            var invoice = await _financeRepo.GetInvoiceByIdAsync(dto.InvoiceId)
                ?? throw new InvalidOperationException("Invoice not found.");

            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new InvalidOperationException("Cannot record a payment against a cancelled invoice.");

            var applied = invoice.Payments.Where(p => p.Status == PaymentStatus.Verified).Sum(p => p.Amount)
                          + invoice.Waivers.Sum(w => w.Amount);
            var balance = invoice.TotalAmount - applied;
            if (dto.Amount > balance)
                throw new InvalidOperationException($"Amount exceeds the outstanding balance ({balance:N2} ETB).");

            if (!string.IsNullOrWhiteSpace(dto.SlipNumber))
            {
                if (await _financeRepo.IsSlipNumberUsedAsync(dto.SlipNumber.Trim()))
                    throw new InvalidOperationException("This slip number has already been used. Duplicate slips are not allowed.");
            }

            var payment = new Payment
            {
                Reference = $"MAN-{invoice.Id}-{DateTime.UtcNow.Ticks}",
                InvoiceId = invoice.Id,
                StudentId = invoice.StudentId,
                Amount = dto.Amount,
                Method = PaymentMethod.ManualBank,
                Status = PaymentStatus.Pending,
                SlipNumber = dto.SlipNumber?.Trim(),
                PayerName = dto.PayerName?.Trim(),
                Remarks = dto.Remarks?.Trim()
            };

            payment = await _financeRepo.AddPaymentAsync(payment);

            await _auditLogService.LogAsync(officerUserId, "", "Finance", "Record", "Payment", payment.Id,
                $"Recorded manual payment {dto.Amount:N2} ETB on invoice {invoice.InvoiceNumber} (slip {dto.SlipNumber}).");

            return FeeService.MapPayment(payment);
        }

        public async Task VerifyManualPaymentAsync(int paymentId, string officerUserId)
        {
            var payment = await _financeRepo.GetPaymentByIdAsync(paymentId)
                ?? throw new InvalidOperationException("Payment not found.");

            if (payment.Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be verified.");

            await MarkVerifiedAsync(payment, null, officerUserId);

            await _auditLogService.LogAsync(officerUserId, "", "Finance", "Verify", "Payment", payment.Id,
                $"Verified manual payment {payment.Amount:N2} ETB (ref {payment.Reference}).");
        }

        public async Task RejectManualPaymentAsync(int paymentId, string officerUserId, string reason)
        {
            var payment = await _financeRepo.GetPaymentByIdAsync(paymentId)
                ?? throw new InvalidOperationException("Payment not found.");

            if (payment.Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be rejected.");

            payment.Status = PaymentStatus.Failed;
            payment.Remarks = string.IsNullOrWhiteSpace(reason) ? "Rejected by finance officer" : reason.Trim();
            payment.VerifiedAt = DateTime.UtcNow;
            payment.VerifiedByUserId = officerUserId;
            await _financeRepo.UpdatePaymentAsync(payment);

            await _auditLogService.LogAsync(officerUserId, "", "Finance", "Reject", "Payment", payment.Id,
                $"Rejected payment {payment.Amount:N2} ETB: {payment.Remarks}.");
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsAsync(int? semesterId = null, PaymentStatus? status = null)
        {
            var list = await _financeRepo.GetPaymentsAsync(semesterId, status);
            return list.Select(FeeService.MapPayment);
        }

        public async Task RefundPaymentAsync(int paymentId, string reason, string officerUserId)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new InvalidOperationException("A reason is required for refunds.");

            var payment = await _financeRepo.GetPaymentByIdAsync(paymentId)
                ?? throw new InvalidOperationException("Payment not found.");

            if (payment.Status != PaymentStatus.Verified)
                throw new InvalidOperationException("Only verified payments can be refunded.");

            var previousStatus = payment.Status;
            payment.Status = PaymentStatus.Refunded;
            payment.Remarks = $"Refunded: {reason.Trim()}";
            payment.VerifiedAt = DateTime.UtcNow;
            payment.VerifiedByUserId = officerUserId;

            // Re-compute the invoice status now that the payment no longer counts
            // (the tracked payment is part of Invoice.Payments; its status change
            // is picked up immediately).
            if (payment.Invoice != null)
            {
                var applied = payment.Invoice.Payments.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount)
                              + payment.Invoice.Waivers.Sum(w => w.Amount);
                payment.Invoice.Status = applied >= payment.Invoice.TotalAmount ? InvoiceStatus.Paid
                    : applied > 0 ? InvoiceStatus.PartiallyPaid
                    : InvoiceStatus.Unpaid;
            }

            // Reversal posted to the append-only ledger as a new charge (+),
            // putting the money back on the student's balance.
            var reversal = new LedgerEntry
            {
                StudentId = payment.StudentId,
                InvoiceId = payment.InvoiceId,
                PaymentId = payment.Id,
                EntryType = "Adjustment",
                Amount = payment.Amount,
                Description = $"Refund of payment {payment.Reference}: {reason.Trim()}"
            };

            // One atomic save: payment + reversal entry + invoice status.
            await _financeRepo.ApplyRefundAsync(payment, reversal);

            await _auditLogService.LogAsync(officerUserId, "", "Finance", "Refund", "Payment", payment.Id,
                $"Refunded payment {payment.Amount:N2} ETB (ref {payment.Reference}, previously {previousStatus}): {reason.Trim()}.");

            // Notify the student (best effort).
            try
            {
                var student = await _studentRepo.GetStudentWithUserAsync(payment.StudentId);
                if (student?.ApplicationUserId != null)
                {
                    await _notificationService.SendNotificationAsync(
                        student.ApplicationUserId,
                        "Payment Refunded",
                        $"Your payment of {payment.Amount:N2} ETB (ref {payment.Reference}) has been refunded: {reason.Trim()}.",
                        "Finance",
                        payment.Id.ToString(),
                        "/StudentFinance/MyFinance");
                }
            }
            catch
            {
                // Notification failures must never block the refund.
            }
        }

        public async Task<byte[]> ExportInvoicesAsync(int? semesterId, InvoiceStatus? status, string format)
        {
            var rows = await GetInvoiceExportRowsAsync(semesterId, status);
            return IsExcel(format)
                ? FinanceExportHelper.ExportToExcel(rows)
                : FinanceExportHelper.ExportToPdf(rows, "Invoice Report");
        }

        public async Task<byte[]> ExportPaymentsAsync(int? semesterId, PaymentStatus? status, string format)
        {
            var rows = await GetPaymentExportRowsAsync(semesterId, status);
            return IsExcel(format)
                ? FinanceExportHelper.ExportToExcel(rows)
                : FinanceExportHelper.ExportToPdf(rows, "Payment Report");
        }

        private static bool IsExcel(string? format) =>
            string.Equals(format, "excel", StringComparison.OrdinalIgnoreCase);

        public async Task<List<Dictionary<string, object>>> GetInvoiceExportRowsAsync(int? semesterId, InvoiceStatus? status)
        {
            var invoices = await _financeRepo.GetInvoicesAsync(semesterId, status);
            return invoices.Select(i =>
            {
                var paid = i.Payments.Where(p => p.Status == PaymentStatus.Verified).Sum(p => p.Amount);
                var waived = i.Waivers.Sum(w => w.Amount);
                var balance = i.Status == InvoiceStatus.Cancelled ? 0m : i.TotalAmount - paid - waived;
                return new Dictionary<string, object>
                {
                    ["Invoice Number"] = i.InvoiceNumber,
                    ["Student Name"] = i.Student != null ? $"{i.Student.FirstName} {i.Student.LastName}" : "",
                    ["Student ID"] = i.Student?.StudentId ?? "",
                    ["Semester"] = i.Semester?.Name ?? "",
                    ["Category"] = i.PaymentCategory.ToString(),
                    ["Total (ETB)"] = i.TotalAmount.ToString("N2"),
                    ["Paid (ETB)"] = paid.ToString("N2"),
                    ["Waived (ETB)"] = waived.ToString("N2"),
                    ["Balance (ETB)"] = balance.ToString("N2"),
                    ["Status"] = i.Status.ToString(),
                    ["Issued"] = i.CreatedAt.ToString("yyyy-MM-dd")
                };
            }).ToList();
        }

        public async Task<List<Dictionary<string, object>>> GetPaymentExportRowsAsync(int? semesterId, PaymentStatus? status)
        {
            var payments = await _financeRepo.GetPaymentsAsync(semesterId, status);
            return payments.Select(p => new Dictionary<string, object>
            {
                ["Reference"] = p.Reference,
                ["Slip Number"] = p.SlipNumber ?? "",
                ["Student Name"] = p.Student != null ? $"{p.Student.FirstName} {p.Student.LastName}" : "",
                ["Student ID"] = p.Student?.StudentId ?? "",
                ["Invoice"] = p.Invoice?.InvoiceNumber ?? "",
                ["Amount (ETB)"] = p.Amount.ToString("N2"),
                ["Method"] = p.Method.ToString(),
                ["Status"] = p.Status.ToString(),
                ["Recorded"] = p.CreatedAt.ToString("yyyy-MM-dd"),
                ["Verified"] = p.VerifiedAt?.ToString("yyyy-MM-dd") ?? "",
                ["Remarks"] = p.Remarks ?? ""
            }).ToList();
        }

        // ================= Helpers =================

        /// <summary>
        /// Marks a payment verified, posts the ledger credit and updates the
        /// invoice status in ONE atomic save. Idempotent: a verified payment can
        /// never be credited twice.
        /// </summary>
        private async Task MarkVerifiedAsync(Payment payment, string? gatewayTransactionId, string? officerUserId = null)
        {
            if (payment.Status == PaymentStatus.Verified) return; // idempotency guard

            payment.Status = PaymentStatus.Verified;
            payment.VerifiedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(gatewayTransactionId))
                payment.GatewayTransactionId = gatewayTransactionId;
            if (officerUserId != null)
                payment.VerifiedByUserId = officerUserId;

            // The tracked payment is part of Invoice.Payments, so the verified
            // sum below already includes this payment exactly once.
            if (payment.Invoice != null)
            {
                var applied = payment.Invoice.Payments.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount)
                              + payment.Invoice.Waivers.Sum(w => w.Amount);
                payment.Invoice.Status = applied >= payment.Invoice.TotalAmount ? InvoiceStatus.Paid
                    : applied > 0 ? InvoiceStatus.PartiallyPaid
                    : InvoiceStatus.Unpaid;
            }

            var entry = new LedgerEntry
            {
                StudentId = payment.StudentId,
                InvoiceId = payment.InvoiceId,
                PaymentId = payment.Id,
                EntryType = "Payment",
                Amount = -payment.Amount,
                Description = payment.Method == PaymentMethod.Chapa
                    ? $"Gateway payment (ref {payment.Reference})"
                    : $"Bank payment (slip {payment.SlipNumber ?? payment.Reference})"
            };

            // One atomic save: payment + ledger credit + invoice status.
            await _financeRepo.ApplyVerifiedPaymentAsync(payment, entry);

            // Notify the student (best effort - must never fail the payment).
            try
            {
                var student = await _studentRepo.GetStudentWithUserAsync(payment.StudentId);
                if (student != null)
                {
                    if (student.ApplicationUserId != null)
                    {
                        await _notificationService.SendNotificationAsync(
                            student.ApplicationUserId,
                            "Payment Confirmed",
                            $"Your payment of {payment.Amount:N2} ETB has been confirmed. Reference: {payment.Reference}.",
                            "Finance",
                            payment.Id.ToString(),
                            "/StudentFinance/MyFinance");
                    }

                    // Email receipt (best-effort; skipped when SMTP is not configured).
                    if (!string.IsNullOrWhiteSpace(student.Email))
                    {
                        await _emailSender.SendAsync(
                            student.Email,
                            $"{student.FirstName} {student.LastName}",
                            $"Payment Confirmed - {payment.Amount:N2} ETB (Ref {payment.Reference})",
                            BuildReceiptEmailHtml(student, payment));
                    }
                }
            }
            catch
            {
                // Notification/email failures must never block a verified payment.
            }
        }

        private static string BuildReceiptEmailHtml(Student student, Payment payment)
        {
            var invoiceNumber = payment.Invoice?.InvoiceNumber ?? "-";
            var method = payment.Method == PaymentMethod.Chapa ? "Online (Chapa)" : "Bank / Slip";
            var slip = string.IsNullOrWhiteSpace(payment.SlipNumber) ? "" : $"<tr><td style='padding:4px 12px;color:#6c757d;'>Slip Number</td><td style='padding:4px 12px;font-weight:600;'>{System.Net.WebUtility.HtmlEncode(payment.SlipNumber)}</td></tr>";

            return $@"
<div style='font-family:Segoe UI,Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #e9ecef;border-radius:8px;overflow:hidden;'>
    <div style='background:#0d6efd;color:#fff;padding:20px 24px;'>
        <h2 style='margin:0;font-size:18px;'>University Management System</h2>
        <span style='opacity:.85;font-size:13px;'>Official Payment Confirmation</span>
    </div>
    <div style='padding:24px;'>
        <p>Dear {System.Net.WebUtility.HtmlEncode(student.FirstName)},</p>
        <p>Your payment has been confirmed and posted to your student account. This email serves as your official receipt.</p>
        <table style='width:100%;border-collapse:collapse;font-size:14px;margin:16px 0;'>
            <tr><td style='padding:4px 12px;color:#6c757d;'>Amount</td><td style='padding:4px 12px;font-weight:600;color:#198754;'>{payment.Amount:N2} ETB</td></tr>
            <tr><td style='padding:4px 12px;color:#6c757d;'>Reference</td><td style='padding:4px 12px;font-weight:600;'>{System.Net.WebUtility.HtmlEncode(payment.Reference)}</td></tr>
            <tr><td style='padding:4px 12px;color:#6c757d;'>Invoice</td><td style='padding:4px 12px;'>{System.Net.WebUtility.HtmlEncode(invoiceNumber)}</td></tr>
            <tr><td style='padding:4px 12px;color:#6c757d;'>Method</td><td style='padding:4px 12px;'>{method}</td></tr>
            {slip}
            <tr><td style='padding:4px 12px;color:#6c757d;'>Date</td><td style='padding:4px 12px;'>{payment.VerifiedAt:MMM dd, yyyy HH:mm} UTC</td></tr>
        </table>
        <p style='font-size:13px;color:#6c757d;'>You can view your full statement and print a receipt from the My Finance page of the student portal.</p>
        <p style='font-size:13px;color:#6c757d;margin-bottom:0;'>If you did not make this payment, please contact the university finance office immediately.</p>
    </div>
</div>";
        }

        private static string? GetChapaSecretKey() =>
            Environment.GetEnvironmentVariable("CHAPA_SECRET_KEY");

        private static string? GetChapaPublicKey() =>
            Environment.GetEnvironmentVariable("CHAPA_PUBLIC_KEY");
    }
}
