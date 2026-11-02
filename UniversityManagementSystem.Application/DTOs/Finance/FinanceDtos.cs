using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Finance
{
    public class FeeStructureDto
    {
        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal AmountForGovernmentSponsored { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class FeeStructureCreateDto
    {
        public int AcademicYearId { get; set; }
        public int DepartmentId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal AmountForGovernmentSponsored { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public PaymentCategory PaymentCategory { get; set; }
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public string AcademicYearName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal WaivedAmount { get; set; }
        public decimal Balance { get; set; }
        public DateTime? DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<InvoiceLineDto> Lines { get; set; } = new();
        public List<PaymentDto> Payments { get; set; } = new();
        public List<WaiverDto> Waivers { get; set; } = new();
    }

    public class InvoiceLineDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class PaymentDto
    {
        public int Id { get; set; }
        public string Reference { get; set; } = string.Empty;
        public int InvoiceId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string? SlipNumber { get; set; }
        public string? PayerName { get; set; }
        public string? Remarks { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? InvoiceNumber { get; set; }
    }

    public class WaiverDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime GrantedAt { get; set; }
    }

    /// <summary>Result of the registration clearance check.</summary>
    public class FinanceClearanceResult
    {
        public bool IsCleared { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaymentCategory Category { get; set; }
        public decimal Balance { get; set; }
        public decimal RequiredAmount { get; set; }
    }

    /// <summary>Student-facing finance summary.</summary>
    public class StudentFinanceDto
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public PaymentCategory PaymentCategory { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalWaived { get; set; }
        public decimal Balance { get; set; }
        public List<InvoiceDto> Invoices { get; set; } = new();
        public List<LedgerEntryDto> Ledger { get; set; } = new();
    }

    public class LedgerEntryDto
    {
        public DateTime CreatedAt { get; set; }
        public string EntryType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? InvoiceNumber { get; set; }
    }

    /// <summary>Admin dashboard counters.</summary>
    public class FinanceDashboardDto
    {
        public decimal ExpectedAmount { get; set; }
        public decimal CollectedAmount { get; set; }
        public decimal WaivedAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public int InvoiceCount { get; set; }
        public int PendingPaymentCount { get; set; }
        public List<InvoiceDto> RecentInvoices { get; set; } = new();
        public List<PaymentDto> PendingPayments { get; set; } = new();
    }

    public class ManualPaymentCreateDto
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string SlipNumber { get; set; } = string.Empty;
        public string? PayerName { get; set; }
        public string? Remarks { get; set; }
    }

    /// <summary>Finance reports page data: collection trends per semester and per department.</summary>
    public class FinanceAnalyticsDto
    {
        public int? AcademicYearId { get; set; }
        public List<AcademicYearOption> AcademicYears { get; set; } = new();

        /// <summary>Per-semester collection: expected, collected, waived, outstanding.</summary>
        public List<SemesterCollectionDto> SemesterCollections { get; set; } = new();

        /// <summary>Per-department totals for the selected year (or all years).</summary>
        public List<DepartmentCollectionDto> DepartmentCollections { get; set; } = new();

        /// <summary>Payment counts by status for the selected scope.</summary>
        public int PendingCount { get; set; }
        public int VerifiedCount { get; set; }
        public int FailedCount { get; set; }
        public int RefundedCount { get; set; }

        public decimal TotalExpected { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalWaived { get; set; }
        public decimal TotalOutstanding => Math.Max(0, TotalExpected - TotalCollected - TotalWaived);

        /// <summary>Overall collection rate in percent (0-100).</summary>
        public decimal CollectionRate => TotalExpected <= 0 ? 0 : Math.Round(TotalCollected / TotalExpected * 100m, 1);
    }

    public class AcademicYearOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class SemesterCollectionDto
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public string AcademicYearName { get; set; } = string.Empty;
        public decimal Expected { get; set; }
        public decimal Collected { get; set; }
        public decimal Waived { get; set; }
        public decimal Outstanding => Math.Max(0, Expected - Collected - Waived);
    }

    public class DepartmentCollectionDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public decimal Expected { get; set; }
        public decimal Collected { get; set; }
        public decimal Outstanding => Math.Max(0, Expected - Collected);
    }
}
