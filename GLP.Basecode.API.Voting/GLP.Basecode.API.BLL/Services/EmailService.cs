using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace GLP.Basecode.API.BLL.Services
{
    public class EmailService
    {
        private readonly MailSettings _settings;

        public EmailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<(bool Success, string Message)> SendEmailAsync(string subject, string htmlBody, string recipient = null!)
        {
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.MailSender),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                message.To.Add(recipient);

                using var smtp = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_settings.MailSender, _settings.MailSenderAppPassword),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(message);
                return (true, $"We've sent a 5 digit OTP code to "); //Handle TempData in front-End side.
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }

    public class MailSettings
    {
        public string MailSender { get; set; } = null!;
        public string MailSenderAppPassword { get; set; } = null!;
        public string SmtpHost { get; set; } = null!;
        public int SmtpPort { get; set; }
    }
}