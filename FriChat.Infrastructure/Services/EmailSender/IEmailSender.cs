namespace FriChat.Infrastructure.Services.EmailSender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
