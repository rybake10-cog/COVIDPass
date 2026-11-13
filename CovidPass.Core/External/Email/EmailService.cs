// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CovidPass.Core.External.Email
{
    public interface IEmailService
    {
        Task Send(string to, string subject, string body);
    }
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;

        public EmailService(ILogger<EmailService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task Send(string to, string subject, string body)
        {
            try
            {
                var emailServer = _config.GetValue<string>("EmailServer");
                var emailFrom = _config.GetValue<string>("EmailFrom");
                using (var sc = new SmtpClient(emailServer))
                {
                    sc.UseDefaultCredentials = true;
                    sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                    await sc.SendMailAsync(new MailMessage(emailFrom, to)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Can't send email: " + e);
            }
        }
    }
}
