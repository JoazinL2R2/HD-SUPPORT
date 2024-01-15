using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace HD_SUPPORT.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("testejoazinl2r2@hotmail.com", "admin123Empl@yer")
            };

            return client.SendMailAsync(
                new MailMessage(from: "testejoazinl2r2@hotmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
