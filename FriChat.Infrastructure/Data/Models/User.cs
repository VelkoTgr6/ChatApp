using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static FriChat.Infrastructure.Constants.ModelConstants;

namespace FriChat.Infrastructure.Data.Models
{
    public class User
    {
        [Key]
        [Comment("User Identifier")]
        public string Id { get; set; } = string.Empty;

        [Required]
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

        [Comment("Shows if student is Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [Comment("Email of the user")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Comment("User Identifier")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual IdentityUser TheUser { get; set; } = null!;

        [MaxLength(ProfilePictureMaxLength)]
        [Comment("Path to the profile picture of the student")]
        public string ProfilePicturePath { get; set; } = "images/profiles/default.jpg";

        [Required]
        [Comment("The date of creation of the user")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Comment("The date of last login of the user")]
        public DateTime LastLogin { get; set; }
    }
}
