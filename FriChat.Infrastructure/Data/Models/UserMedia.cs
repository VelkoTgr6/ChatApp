using FriChat.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriChat.Infrastructure.Data.Models
{
    public class UserMedia
    {
        [Key]
        [Comment("Unique identifier for the user media")]
        public int Id { get; set; }

        [Required]
        [MaxLength(2000)]
        [Comment("URL of the media file uploaded by the user")]
        public string Url { get; set; } = string.Empty;

        [Required]
        [Comment("Type of the media file (e.g., image, video)")]
        public MessageType Type { get; set; }

        [Required]
        [Comment("Timestamp when the media was uploaded")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Comment("Identifier for the user who uploaded the media")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [Comment("Navigation property to the user who uploaded the media")]
        public virtual AppUser User { get; set; } = null!;

        [Required]
        [Comment("Identifier for the conversation associated with the media")]
        public int ConversationId { get; set; }

        [ForeignKey(nameof(ConversationId))]
        [Comment("Navigation property to the conversation associated with the media")]
        public virtual Conversation Conversation { get; set; } = null!;
    }
}
