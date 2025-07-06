using FriChat.Infrastructure.Enums;

namespace FriChat.Core.Models.AppUser
{
    public class MessageViewModel
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public int ReceiverUserId { get; set; }
        public int SenderUserId { get; set; }
        public string? UserProfilePicturePath { get; set; }
        public string? ReceiverProfilePicturePath { get; set; }
        public string SenderUserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public MessageType ? AttachmentType { get; set; }  // e.g., "image", "file", "video"
        public string? AttachmentUrl { get; set; }  // Path or URL to the attachment
    }
}
