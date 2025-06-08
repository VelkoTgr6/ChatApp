using FriChat.Infrastructure.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static FriChat.Infrastructure.Constants.ModelConstants;

namespace FriChat.Infrastructure.Data.Models
{
    public class AppUser
    {
        [Key]
        [Comment("User Identifier (same as IdentityUser Id)")]
        public int Id { get; set; } 

        [Required]
        [MaxLength(UserNameMaxLength)]
        [Comment("Username of the User")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(UserNameMaxLength)]
        [Comment("First Name of the User")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(UserNameMaxLength)]
        [Comment("Last Name of the User")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Comment("Date of birth of User")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Comment("The gender of the User")]
        public Gender Gender { get; set; }

        [Comment("Shows if student is Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [Comment("Email of the user")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(ProfilePictureMaxLength)]
        [Comment("Path to the profile picture of the student")]
        public string ProfilePicturePath { get; set; } = "images/profiles/default.jpg";

        [Comment("The date of creation of the user")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Comment("The date of last login of the user")]
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        [Required]
        [Comment("Identity User Id for the user, used for authentication")]
        public string IdentityUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(IdentityUserId))]
        public virtual IdentityUser User { get; set; } = null!;

        [Comment("Collection of messages sent by the user")]
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();

        [Comment("Collection of messages received by the user")]
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        [Comment("Collection of friends of the user")]
        public virtual ICollection<AppUser> Friends { get; set; } = new List<AppUser>();

        [Comment("Collection of the users who have added this user as a friend.")]
        public virtual ICollection<AppUser> FriendOf { get; set; } = new List<AppUser>();

        [Comment("Collection of friend requests sent by the user")]
        public virtual ICollection<AppUser> FriendRequests { get; set; } = new List<AppUser>();

        [Comment("Collection of friend requests received by the user")]
        public virtual ICollection<AppUser> ReceivedFriendRequests { get; set; } = new List<AppUser>();
    }

}
