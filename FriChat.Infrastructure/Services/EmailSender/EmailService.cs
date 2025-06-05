using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;

namespace FriChat.Infrastructure.Services.EmailSender
{
    public class EmailService : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        // Constructor accepting SMTP settings directly
        public EmailService(string smtpServer, int smtpPort, string smtpUser, string smtpPass)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPass = smtpPass;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("EmailServiceTEST", _smtpUser));
            message.To.Add(new MailboxAddress("emailTESTT", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public static string GenerateTemporaryPassword(int length = 8)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

            return new PasswordHasher<IdentityUser>().HashPassword(null,
                new string(Enumerable.Repeat(validChars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray()));
        }
    }
}
