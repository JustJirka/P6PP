using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using NotificationService.API.Persistence;

namespace NotificationService.API.Services
{
    public class MailAppService
    {
        public class SmtpSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool EnableSsl { get; set; }
            public string From { get; set; }
        }

        private readonly SmtpSettings _smtpSettings;

        public MailAppService(IConfiguration configuration)
        {
            _smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
        }

        public async Task SendEmailAsync(EmailArgs emailArgs)
        {

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.From),
                Subject = emailArgs.Subject,
                Body = emailArgs.Body,
                IsBodyHtml = true,
            };
            foreach (var to in emailArgs.Address)
            {
                mailMessage.To.Add(to);
            }

            if (!mailMessage.To.Any())
            {
                return;
            }

            using (var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                smtpClient.EnableSsl = _smtpSettings.EnableSsl;
                await smtpClient.SendMailAsync(mailMessage);

            }
            
        }
    }
}
