using UniversityManagementSystem.Application.DTOs.Finance;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    /// <summary>
    /// Finance data access: fee structures (price book), invoices, payments,
    /// waivers and the append-only ledger. Aggregates are computed in the
    /// database so reports stay correct regardless of row counts, and financial
    /// mutations are exposed as atomic operations (one SaveChanges per business
    /// transaction) so a crash can never leave money recorded without its
    /// ledger entry or vice versa.
    /// </summary>
    public interface IFinanceRepository
    {
        // Fee structures
        Task<IEnumerable<FeeStructure>> GetFeeStructuresAsync(int? academicYearId = null, int? departmentId = null);
        Task<FeeStructure?> GetFeeStructureByIdAsync(int id);
        Task<FeeStructure> AddFeeStructureAsync(FeeStructure entity);
        Task UpdateFeeStructureAsync(FeeStructure entity);
        Task DeleteFeeStructureAsync(FeeStructure entity);

        Task<bool> IsFeeStructureInUseAsync(int feeStructureId);

        // Invoices
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<Invoice?> GetInvoiceByStudentAndSemesterAsync(int studentId, int semesterId);
        Task<IEnumerable<Invoice>> GetInvoicesAsync(int? semesterId = null, InvoiceStatus? status = null, string? search = null, int take = 300);
        Task<List<Invoice>> GetInvoicesByStudentAsync(int studentId);
        Task<int> CountInvoicesAsync(int? semesterId = null);
        Task<int> CountInvoicesByNumberPrefixAsync(string prefix);
        Task<Invoice> AddInvoiceAsync(Invoice invoice);
        Task UpdateInvoiceAsync(Invoice invoice);

        /// <summary>Atomically saves a new invoice together with its ledger entries.</summary>
        Task<Invoice> AddInvoiceWithEntriesAsync(Invoice invoice, IEnumerable<LedgerEntry> entries);

        // Payments
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<Payment?> GetPaymentByReferenceAsync(string reference);
        Task<IEnumerable<Payment>> GetPaymentsAsync(int? semesterId = null, PaymentStatus? status = null);
        Task<Payment> AddPaymentAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);

        Task<bool> IsSlipNumberUsedAsync(string slipNumber);

        /// <summary>
        /// Atomically marks a payment verified, posts its ledger credit and
        /// updates the invoice status in a single save.
        /// </summary>
        Task ApplyVerifiedPaymentAsync(Payment payment, LedgerEntry ledgerEntry);

        /// <summary>Atomically marks a payment refunded, posts the reversal charge and updates the invoice.</summary>
        Task ApplyRefundAsync(Payment payment, LedgerEntry reversalEntry);

        // Waivers
        /// <summary>Atomically saves a waiver with its ledger entry and updated invoice status.</summary>
        Task ApplyWaiverAsync(Waiver waiver, LedgerEntry ledgerEntry);

        // Ledger
        Task AddLedgerEntryAsync(LedgerEntry entry);
        Task AddLedgerEntriesAsync(IEnumerable<LedgerEntry> entries);
        Task<decimal> GetBalanceAsync(int studentId);
        Task<IEnumerable<LedgerEntry>> GetLedgerAsync(int studentId);

        /// <summary>Atomically cancels an invoice and posts the balance reversal entry.</summary>
        Task ApplyInvoiceCancellationAsync(Invoice invoice, LedgerEntry reversalEntry);

        // Aggregates (computed in the database - never load full tables into memory)
        Task<decimal> SumInvoiceTotalsAsync(int? semesterId = null, int? academicYearId = null);
        Task<decimal> SumVerifiedPaymentsAsync(int? semesterId = null, int? academicYearId = null);
        Task<decimal> SumWaiversAsync(int? semesterId = null, int? academicYearId = null);
        Task<int> CountPaymentsAsync(PaymentStatus status);
        Task<Dictionary<PaymentStatus, int>> GetPaymentStatusCountsAsync(int? academicYearId);
        Task<List<SemesterCollectionDto>> GetSemesterCollectionsAsync(int? academicYearId);
        Task<List<DepartmentCollectionDto>> GetDepartmentCollectionsAsync(int? academicYearId);
    }
}
