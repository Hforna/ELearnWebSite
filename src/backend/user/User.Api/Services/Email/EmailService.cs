using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace User.Api.Services.Email
{
    public class EmailService
    {
        private readonly string _email;
        private readonly string _password;
        private readonly string _name;

        public EmailService(string email, string password, string name)
        {
            _email = email;
            _password = password;
            _name = name;
        }

        public async Task SendEmail(string customerEmail, string name, string subject, string text)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_name, _email));
            message.To.Add(new MailboxAddress(name, customerEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = @$"<!DOCTYPE html>
                <html>
                <body style=""font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; margin: 0; padding: 20px;"">

                    <div style=""max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);"">
                        <h2 style=""color: #4CAF50; text-align: center;"">Hi, {name}!</h2>
                        <p style=""font-size: 16px;"">{text}.</p>
                    </div>
                </body>
                </html>" };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(_email, _password);

                await client.SendAsync(message);

                client.Disconnect(true);
            }
        }
    }
}
