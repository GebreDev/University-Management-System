using UniversityManagementSystem.Domain.Common;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// Single-row finance configuration, fully controlled by the Admin.
    /// </summary>
    public class FinanceSetting : BaseEntity
    {
        /// <summary>Master switch: enforce payment before semester registration.</summary>
        public bool RegistrationGateEnabled { get; set; } = true;

        /// <summary>Percent of the invoice a self-sponsored student must cover before registering (e.g. 50).</summary>
        public decimal FirstInstallmentPercent { get; set; } = 50m;

        /// <summary>Base URL used to build Chapa callback/return URLs (e.g. https://ums.example.com).</summary>
        public string ChapaBaseUrl { get; set; } = string.Empty;
    }
}
