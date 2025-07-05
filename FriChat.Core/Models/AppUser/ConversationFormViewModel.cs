namespace FriChat.Core.Models.AppUser
{
    public class ConversationFormViewModel
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public int ReceiverUserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ReceiverUserName { get; set; } = string.Empty;
        public string ConversationName { get; set; } = string.Empty;
        public string? ConversationPicturePath { get; set; }
        public string? UserProfilePicturePath { get; set; }
        public string? ReceiverProfilePicturePath { get; set; }
        public MessageViewModel ? NewMessage { get; set; } = new MessageViewModel();
        public bool IsGroupConversation { get; set; } = false;
        public List<MessageViewModel> ? Messages { get; set; } = new();
        //public DateTime LastMessageTimestamp => Messages.Any() ? Messages.Last().Timestamp : DateTime.MinValue;
        public bool LastMessageSeen => Messages.Any() ? Messages.Last().IsRead : true;
    }
}
