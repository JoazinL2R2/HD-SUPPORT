using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace HD_SUPPORT.Services
{
    public interface IEmailService
    {
        public bool Enviar(string email, string assunto, string mensagem);
    }

    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Enviar(string email, string assunto, string mensagem)
        {
            try
            {
                string host = _configuration.GetValue<string>("SMTP:Host");
                string nome = _configuration.GetValue<string>("SMTP:Nome");
                string username = _configuration.GetValue<string>("SMTP:UserName");
                string senha = _configuration.GetValue<string>("SMTP:Senha");
                int porta = _configuration.GetValue<int>("SMTP:Porta");

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress( username, nome)
                };
                mail.To.Add(email);
                mail.Subject = assunto;
                mail.Body = mensagem;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(host, porta))
                {
                    smtp.Credentials = new NetworkCredential(username, senha); // Fornecer credenciais
                    smtp.EnableSsl = true;

                    smtp.Send(mail);

                    return true;
                }
            }
            catch (SystemException ex)
            {
                // Trate a exceção ou registre para depuração
                return false;
            }
        }
    }
}
