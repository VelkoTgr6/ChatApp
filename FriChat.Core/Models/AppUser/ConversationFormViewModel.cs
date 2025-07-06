namespace FriChat.Core.Models.AppUser
{
    public class ConversationFormViewModel
    {
        public int ConversationId { get; set; }
        public int ReceiverUserId { get; set; }
        public string ReceiverUserName { get; set; } = string.Empty;
        public string ConversationName { get; set; } = string.Empty;
        public string? ConversationPicturePath { get; set; }
        public string? UserProfilePicturePath { get; set; }
        public string? ReceiverProfilePicturePath { get; set; }
        public bool IsGroupConversation { get; set; } = false;
    }
}
