using MailKit.Net.Smtp;
using MimeKit;

namespace FriChat.Infrastructure.Services.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        // Constructor accepting SMTP settings directly
        public EmailSender(string smtpServer, int smtpPort, string smtpUser, string smtpPass)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPass = smtpPass;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TEST MAIL", _smtpUser));
            message.To.Add(new MailboxAddress("Recipient", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html") // Use "html" if your body contains HTML
            {
                Text = body
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    // The second parameter is "useSsl"; true means to use SSL
                    await client.ConnectAsync(_smtpServer, _smtpPort, true);
                    await client.AuthenticateAsync(_smtpUser, _smtpPass);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"An error occurred while sending email: {ex.Message}");
                throw; // Rethrow or handle according to your error strategy
            }
        
         }
    }
}
