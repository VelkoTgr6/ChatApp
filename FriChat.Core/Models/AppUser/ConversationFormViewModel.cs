namespace FriChat.Core.Models.AppUser
{
    public class ConversationFormViewModel
    {
        public int UserId { get; set; }
        public int ReceiverUserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ReceiverUserName { get; set; } = string.Empty;
    }
}
