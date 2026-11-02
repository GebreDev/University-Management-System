namespace UniversityManagementSystem.Domain.Enums
{
    public enum InvoiceStatus
    {
        /// <summary>Generated but nothing applied/paid yet.</summary>
        Unpaid = 1,

        /// <summary>Fully covered by payments/waivers.</summary>
        Paid = 2,

        /// <summary>Some amount applied.</summary>
        PartiallyPaid = 3,

        /// <summary>Cancelled (never posted to ledger); kept for audit.</summary>
        Cancelled = 4
    }

    public enum PaymentStatus
    {
        /// <summary>Gateway checkout started, not confirmed yet.</summary>
        Pending = 1,

        /// <summary>Confirmed by gateway webhook (or manually verified by finance).</summary>
        Verified = 2,

        Failed = 3,

        Cancelled = 4,

        Refunded = 5
    }

    public enum PaymentMethod
    {
        /// <summary>Online via Chapa (telebirr, CBE Birr, cards...).</summary>
        Chapa = 1,

        /// <summary>Bank slip / counter payment recorded and verified by a finance officer.</summary>
        ManualBank = 2
    }
}
