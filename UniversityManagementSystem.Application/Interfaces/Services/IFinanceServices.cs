using UniversityManagementSystem.Application.DTOs.Finance;
using UniversityManagementSystem.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    /// <summary>
    /// Fee assessment, waivers, ledger balance and registration clearance.
    /// </summary>
    public interface IFeeService
    {
        /// <summary>
        /// Generates (or returns the existing) invoice for a student's semester
        /// registration, snapshots the active fee structure and auto-waives
        /// government-sponsored coverage.
        /// </summary>
        Task<InvoiceDto> AssessSemesterAsync(int studentId, int semesterId);

        Task<FinanceClearanceResult> CheckRegistrationClearanceAsync(int studentId, int semesterId);

        Task<StudentFinanceDto> GetStudentFinanceAsync(int studentId);
        Task<InvoiceDto?> GetInvoiceAsync(int invoiceId);

        // Admin
        Task<IEnumerable<FeeStructureDto>> GetFeeStructuresAsync(int? academicYearId = null, int? programId = null);
        Task<FeeStructureDto> CreateFeeStructureAsync(FeeStructureCreateDto dto, string adminUserId);
        Task UpdateFeeStructureAsync(int id, FeeStructureCreateDto dto, string adminUserId);
        Task DeleteFeeStructureAsync(int id, string adminUserId);
        Task GrantWaiverAsync(int invoiceId, decimal amount, string reason, string adminUserId);
        Task CancelInvoiceAsync(int invoiceId, string adminUserId);

        Task<FinanceDashboardDto> GetDashboardAsync(int? academicYearId);

        /// <summary>Reports data: collection trends per semester and per department.</summary>
        Task<FinanceAnalyticsDto> GetFinanceAnalyticsAsync(int? academicYearId);
    }

    /// <summary>
    /// Payment recording, gateway (Chapa) initialization, webhook verification
    /// and manual finance-officer verification.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>Starts a Chapa checkout for the invoice balance and returns the checkout URL.</summary>
        Task<string> InitializeChapaPaymentAsync(int studentId, int invoiceId, string returnUrlBase);

        /// <summary>Verifies a gateway callback by tx_ref (used by the return URL as a fallback).</summary>
        Task<bool> VerifyByReferenceAsync(string txRef);

        /// <summary>Handles the Chapa webhook payload (charge ID + tx_ref) with signature verification.</summary>
        Task<bool> HandleWebhookAsync(string chapaChargeId, string txRef, string signature);

        /// <summary>
        /// True when no Chapa keys are configured: online payments run against a
        /// local mock checkout so the flow can be tested without real money.
        /// Setting CHAPA_SECRET_KEY / CHAPA_PUBLIC_KEY switches to the real
        /// gateway automatically.
        /// </summary>
        bool IsMockMode { get; }

        /// <summary>
        /// TEST MODE ONLY: confirms a pending gateway payment locally without
        /// calling Chapa. Guarded by student ownership + pending status.
        /// </summary>
        Task<bool> ConfirmMockPaymentAsync(int paymentId, int studentId);

        /// <summary>Finance officer records a manual bank/slip payment (goes to Pending for verification).</summary>
        Task<PaymentDto> RecordManualPaymentAsync(ManualPaymentCreateDto dto, string officerUserId);

        /// <summary>Finance officer verifies a pending manual payment and posts it to the ledger.</summary>
        Task VerifyManualPaymentAsync(int paymentId, string officerUserId);

        Task RejectManualPaymentAsync(int paymentId, string officerUserId, string reason);

        Task<IEnumerable<PaymentDto>> GetPaymentsAsync(int? semesterId = null, PaymentStatus? status = null);

        /// <summary>
        /// Refunds a verified payment: marks it Refunded and posts an offsetting
        /// ledger charge so the student's balance goes back up. The original
        /// payment row is never deleted - history stays intact.
        /// </summary>
        Task RefundPaymentAsync(int paymentId, string reason, string officerUserId);

        /// <summary>Invoice report as a downloadable file ("excel" or "pdf").</summary>
        Task<byte[]> ExportInvoicesAsync(int? semesterId, InvoiceStatus? status, string format);

        /// <summary>Payment report as a downloadable file ("excel" or "pdf").</summary>
        Task<byte[]> ExportPaymentsAsync(int? semesterId, PaymentStatus? status, string format);
    }
}
