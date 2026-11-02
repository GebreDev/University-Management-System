using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    /// <summary>
    /// Outgoing email abstraction. Implementations must be best-effort from the
    /// caller's perspective: business flows (payments, verification) must never
    /// fail because an email could not be sent.
    /// </summary>
    public interface IEmailSenderService
    {
        /// <summary>Sends an email. Returns true when the send (or the skip when unconfigured) succeeded.</summary>
        Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlBody);
    }
}
