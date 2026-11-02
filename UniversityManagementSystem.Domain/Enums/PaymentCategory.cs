namespace UniversityManagementSystem.Domain.Enums
{
    /// <summary>
    /// Who is financially responsible for a student's tuition.
    /// Assigned by the registrar/finance at admission; re-confirmed each academic year.
    /// </summary>
    public enum PaymentCategory
    {
        /// <summary>Tuition covered by government sponsorship (fees assessed then auto-waived).</summary>
        GovernmentSponsored = 1,

        /// <summary>Student pays tuition personally (via gateway or manual bank payment).</summary>
        SelfSponsored = 2
    }
}
