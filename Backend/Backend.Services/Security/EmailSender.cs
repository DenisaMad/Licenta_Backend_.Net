using Backend.DataAbstraction.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using Backend.DataAbstraction;

namespace Backend.Services.Security
{
    public class EmailSender : IEmailSender
    {
        private readonly IEmailConfig _configuration;

        public EmailSender(IEmailConfig configuration)
        {
            _configuration = configuration;

        }
        public void SendEmail(string to, string subject)
        {
            var fromAddress = _configuration.From;
            var smtpHost = _configuration.Host;
            var smtpPort = _configuration.Port;
            var username = _configuration.Username;
            var password = _configuration.Password;

            var message = new MailMessage
            {
                From = new MailAddress(fromAddress),
                Subject = subject
            };

            message.To.Add(to);

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            client.Send(message);
        }
    }


    }

