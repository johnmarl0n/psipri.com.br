using Microsoft.AspNetCore.Mvc;
using psipri.com.br.Services;
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
        /// Handles the contact form submission.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string name, string email, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                return RedirectToAction("Index", "Home");
            }

            string subject = $"Novo Contato do Site: {name}";
            string body = $"<p><strong>Nome:</strong> {name}</p>" +
                          $"<p><strong>E-mail:</strong> {email}</p>" +
                          $"<p><strong>Mensagem:</strong><br/>{message}</p>";

            try
            {
                // Send email to the psychologist
                string recipient = _config["EmailSettings:Recipient"] ?? _config["EmailSettings:Username"];
                await _emailService.SendEmailAsync(recipient, subject, body);
                
                // Optional: Send auto-reply to the user
                // await _emailService.SendEmailAsync(email, "Recebemos sua mensagem", "<p>Obrigada pelo contato. Em breve retornaremos.</p>");
                
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
