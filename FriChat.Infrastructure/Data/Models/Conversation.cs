using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriChat.Infrastructure.Data.Models
{
    public class Conversation
    {
        [Key]
        [Comment("Unique identifier for the conversation")]
        public int Id { get; set; } 

        [Required]
        [Comment("Identifier for the user who initiated the conversation")]
        public int UserId { get; set; } 

        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; } = null!; 

        [Required]
        [Comment("Identifier for the user receiving the conversation")]
        public int ReceiverUserId { get; set; } 

        [ForeignKey(nameof(ReceiverUserId))]
        public virtual AppUser ReceiverUser { get; set; } = null!; 

        [Comment("Title of the conversation")]
        public string Title { get; set; } = string.Empty; 

        [Required]
        [Comment("Username of the user who initiated the conversation")]
        public string UserName { get; set; } = string.Empty; 

        [Required]
        [Comment("Username of the user receiving the conversation")]
        public string ReceiverUserName { get; set; } = string.Empty; 

        [Comment("Timestamp when the conversation was created")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        [Comment("Timestamp of the last message in the conversation")]
        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow; 

        [Comment("Flag indicating if the conversation is deleted")]
        public bool IsDeleted { get; set; } = false; 

        [Comment("Indicates if the conversation is a group chat")]
        public bool IsGroupConversation { get; set; } = false;

        [Comment("Collection of messages in the conversation")]
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>(); 

        [NotMapped]
        public int UnreadMessageCount => Messages.Count(m => !m.IsRead && m.ReceiverId == ReceiverUserId);

        [NotMapped]
        public Message? LastMessage => Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault();

        //public void MarkAllAsRead(int receiverId)
        //{
        //    foreach (var message in Messages.Where(m => m.ReceiverId == receiverId && !m.IsRead))
        //    {
        //        message.IsRead = true;
        //    }
        //}
    }
}
