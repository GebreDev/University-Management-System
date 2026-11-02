using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// An invoice issued to one student for one semester. Immutable once
    /// payments/waivers are applied; corrections go through credit adjustments.
    /// The student's balance is always derived from ledger entries, never stored here.
    /// </summary>
    public class Invoice : BaseEntity
    {
        /// <summary>Human-friendly number, e.g. INV-2026-000123.</summary>
        public string InvoiceNumber { get; set; } = string.Empty;

        public int StudentId { get; set; }
        public virtual Student? Student { get; set; }

        public int SemesterId { get; set; }
        public virtual Semester? Semester { get; set; }

        /// <summary>Category snapshot at assessment time (category changes later don't rewrite history).</summary>
        public PaymentCategory PaymentCategory { get; set; }

        public decimal TotalAmount { get; set; }

        /// <summary>Semester-level due date (default: 30 days after invoice date).</summary>
        public DateTime? DueDate { get; set; }

        public new InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;

        public string? Remarks { get; set; }

        public virtual ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Waiver> Waivers { get; set; } = new List<Waiver>();
    }

    /// <summary>
    /// One charge line on an invoice. Amount is snapshotted from the fee structure.
    /// </summary>
    public class InvoiceLine : BaseEntity
    {
        public int InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        /// <summary>Fee structure this line came from (for reporting).</summary>
        public int? FeeStructureId { get; set; }
        public virtual FeeStructure? FeeStructure { get; set; }
    }

    /// <summary>
    /// A waiver/discount applied to an invoice: government coverage, scholarship, or
    /// an administrative adjustment. Never deletes the charge - documents why the
    /// student does not pay it.
    /// </summary>
    public class Waiver : BaseEntity
    {
        public int InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }

        public decimal Amount { get; set; }

        /// <summary>e.g. "Government sponsorship", "Registrar override", "Scholarship".</summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>ApplicationUser id of the admin/finance officer who granted it (null for system-generated).</summary>
        public string? GrantedByUserId { get; set; }

        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// A payment against an invoice. Lifecycle: Pending (gateway checkout started,
    /// or slip submitted) -> Verified (webhook confirmed or finance officer verified)
    /// -> posted to ledger. Never deleted.
    /// </summary>
    public class Payment : BaseEntity
    {
        /// <summary>Unique reference: our internal id (tx-ref sent to Chapa) and Chapa's own reference.</summary>
        public string Reference { get; set; } = string.Empty;
        public string? GatewayTransactionId { get; set; }

        public int InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }

        public int StudentId { get; set; }
        public virtual Student? Student { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }

        public new PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        /// <summary>Bank/telebirr slip number for manual payments.</summary>
        public string? SlipNumber { get; set; }

        public string? PayerName { get; set; }

        public string? Remarks { get; set; }

        public DateTime? VerifiedAt { get; set; }

        /// <summary>ApplicationUser id of the finance officer who verified (manual payments).</summary>
        public string? VerifiedByUserId { get; set; }

        /// <summary>Checkout URL returned by the gateway (for resuming an unfinished payment).</summary>
        public string? CheckoutUrl { get; set; }
    }

    /// <summary>
    /// Append-only financial ledger. The student's balance is derived from these
    /// entries: charges (+), payments (-), waivers (-), adjustments (+/-).
    /// Never update or delete entries - corrections are new entries.
    /// </summary>
    public class LedgerEntry : BaseEntity
    {
        public int StudentId { get; set; }
        public virtual Student? Student { get; set; }

        public int? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }

        public int? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }

        public int? WaiverId { get; set; }
        public virtual Waiver? Waiver { get; set; }

        /// <summary>Charge, Payment, Waiver, Adjustment.</summary>
        public string EntryType { get; set; } = string.Empty;

        /// <summary>Positive = charge (debit), negative = credit (payment/waiver).</summary>
        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
