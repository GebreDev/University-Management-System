using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// Price book entry: one fee item (tuition, registration, lab...) for a
    /// department in a specific academic year. Amounts are snapshotted into
    /// invoice lines at assessment time so later price changes never alter
    /// existing invoices.
    /// </summary>
    public class FeeStructure : BaseEntity
    {
        public int AcademicYearId { get; set; }
        public virtual AcademicYear? AcademicYear { get; set; }

        /// <summary>Fees are priced per department/college (the student's owning department).</summary>
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

        /// <summary>Free-text item name, e.g. "Tuition", "Registration", "Lab".</summary>
        public string ItemName { get; set; } = string.Empty;

        /// <summary>Amount in ETB. Applies to self-sponsored students.</summary>
        public decimal Amount { get; set; }

        /// <summary>Optional amount charged to government-sponsored students (e.g. registration fees they still pay). 0 = fully covered.</summary>
        public decimal AmountForGovernmentSponsored { get; set; }

        /// <summary>When the payment becomes overdue (used for the registration gate).</summary>
        public DateTime? DueDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
