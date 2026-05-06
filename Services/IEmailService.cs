using System.Threading.Tasks;

namespace psipri.com.br.Services
{
    /// <summary>
    /// Interface for email delivery services.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="email">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="message">Email body (HTML supported).</param>
        Task SendEmailAsync(string email, string subject, string message);
    }
}
