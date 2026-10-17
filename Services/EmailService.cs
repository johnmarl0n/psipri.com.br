using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace psipri.com.br.Services
{
    /// <summary>
    /// Implementation of IEmailService using MailKit and SMTP (Google account).
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Sends an email via SMTP.
        /// </summary>
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var username = _config["EmailSettings:Username"] 
                ?? throw new InvalidOperationException("EmailSettings:Username não configurado.");
            var host = _config["EmailSettings:Host"] 
                ?? throw new InvalidOperationException("EmailSettings:Host não configurado.");
            var password = _config["EmailSettings:Password"] 
                ?? throw new InvalidOperationException("EmailSettings:Password não configurado.");

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Psipri Notificações", username));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = message };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.CheckCertificateRevocation = false;
            try
            {
                int port = int.Parse(_config["EmailSettings:Port"] ?? "465");
                var security = port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

                await client.ConnectAsync(host, port, security);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // In a real app, log this error
                throw new InvalidOperationException("Falha ao enviar e-mail. Verifique as configurações.", ex);
            }
        }
    }
}
