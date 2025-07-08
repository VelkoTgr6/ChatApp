using FriChat.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static FriChat.Infrastructure.Constants.ModelConstants;

namespace FriChat.Infrastructure.Data.Models
{
    public class Message
    {
        [Key]
        [Comment("Unique identifier for the message")]
        public int Id { get; set; }

        [Required]
        [MaxLength(MessageContentMaxLength)]
        [Comment("Content of the message")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Comment("Timestamp when the message was sent")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Comment("Indicates if the message has been read by the receiver")]
        public bool IsRead { get; set; } = false;

        [Required]
        [Comment("Type of the message (e.g., text, image, file)")]
        public MessageType Type { get; set; } = MessageType.Text;

        [Required]
        [Comment("Identifier of the user who sent the message")]
        public int SenderId { get; set; }

        [Required]
        [Comment("Identifier of the user who received the message")]
        public int ReceiverId { get; set; }

        [Required]
        [Comment("Identifier for the conversation to which this message belongs")]
        public int ConversationId { get; set; }

        [ForeignKey(nameof(ConversationId))]
        [Comment("The conversation to which this message belongs")]
        public virtual Conversation Conversation { get; set; } = null!;

        [Comment("Identifier for the UserMedia to which belongs")]
        public int? UserMediaId { get; set; }

        [ForeignKey(nameof(UserMediaId))]
        [Comment("The media associated with the message, if any (e.g., image, file)")]
        public virtual UserMedia? UserMedia { get; set; } = null;

        [ForeignKey(nameof(SenderId))]
        [Comment("The user who sent the message")]
        public virtual AppUser Sender { get; set; } = null!;

        [ForeignKey(nameof(ReceiverId))]
        [Comment("The user who received the message")]
        public virtual AppUser Receiver { get; set; } = null!;
    }

}
