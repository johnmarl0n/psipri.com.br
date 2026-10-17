using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using psipri.com.br.Services;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace psipri.com.br.Controllers
{
    /// <summary>
    /// Controller for handling contact form submissions.
    /// Uses the EmailService to notify the psychologist.
    /// </summary>
    public class ContactController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public ContactController(IEmailService emailService, IConfiguration config)
        {
            _emailService = emailService;
            _config = config;
        }

        /// <summary>
        /// Handles the contact form submission with sanitization and rate limiting.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("ContactLimiter")]
        public async Task<IActionResult> Send(string name, string email, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                return RedirectToAction("Index", "Home");
            }

            // Sanitize inputs to prevent HTML/Script injection in email notifications
            var safeName = HtmlEncoder.Default.Encode(name.Trim());
            var safeEmail = HtmlEncoder.Default.Encode(email.Trim());
            var safeMessage = HtmlEncoder.Default.Encode(message.Trim()).Replace("\n", "<br/>");

            string subject = $"Novo Contato do Site: {safeName}";
            string body = $"<p><strong>Nome:</strong> {safeName}</p>" +
                          $"<p><strong>E-mail:</strong> {safeEmail}</p>" +
                          $"<p><strong>Mensagem:</strong><br/>{safeMessage}</p>";

            try
            {
                // Send email to the psychologist
                string recipient = _config["EmailSettings:Recipient"] 
                    ?? _config["EmailSettings:Username"] 
                    ?? "priscilabatista.dias@uol.com.br";

                await _emailService.SendEmailAsync(recipient, subject, body);
                
                TempData["MessageSent"] = "Mensagem enviada com sucesso!";
            }
            catch
            {
                TempData["MessageError"] = "Erro ao enviar mensagem. Tente novamente mais tarde.";
            }

            return RedirectToAction("Index", "Home", fragment: "contato");
        }
    }
}
