using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace Course.Application.Services
{
    public class EmailService
    {
        private readonly string _userName;
        private readonly string _email;
        private readonly string _password;

        public EmailService(string userName, string email, string password)
        {
            _userName = userName;
            _email = email;
            _password = password;
        }

        public async Task SendEmail(string toName, string toEmail, string subject, string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_userName, _email));
            message.To.Add(new MailboxAddress(toName, toEmail));

            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = @$"<!DOCTYPE html>
                <html>
                <body style=""font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; margin: 0; padding: 20px;"">

                    <div style=""max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);"">
                        <h2 style=""color: #4CAF50; text-align: center;"">Hi, {toName}!</h2>
                        <p style=""font-size: 16px;"">{body}</p>
                    </div>
                </body>
                </html>"
            };

            using(var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                client.Authenticate(_userName, _password);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }
    }
}
