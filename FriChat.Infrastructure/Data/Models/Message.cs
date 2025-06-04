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
        public Guid Id { get; set; } = Guid.NewGuid();

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
        public string SenderId { get; set; } = string.Empty;

        [Required]
        [Comment("Identifier of the user who received the message")]
        public string ReceiverId { get; set; } = string.Empty;

        [MaxLength(AttachmentUrlMaxLength)]
        [Comment("URL of the attachment, if any (e.g., image, file)")]
        public string? AttachmentUrl { get; set; }

        [ForeignKey(nameof(SenderId))]
        public virtual AppUser Sender { get; set; } = null!;

        [ForeignKey(nameof(ReceiverId))]
        public virtual AppUser Receiver { get; set; } = null!;
    }

}
