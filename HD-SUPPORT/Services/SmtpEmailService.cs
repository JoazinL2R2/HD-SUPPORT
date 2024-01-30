using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace HD_SUPPORT.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private bool _emailEnviado = false;
        private readonly object _lockObject = new object();

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            lock (_lockObject)
            {
                if (_emailEnviado)
                {
                    return;
                }
                _emailEnviado = true;
            }

            try
            {
                var client = new SmtpClient("smtp.office365.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("testejoazinl2r2@hotmail.com", "admin123Empl@yer")
                };

                var mailMessage = new MailMessage(from: "testejoazinl2r2@hotmail.com",
                                                 to: email,
                                                 subject,
                                                 message);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }
    }
}
