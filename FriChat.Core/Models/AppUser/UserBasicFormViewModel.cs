namespace FriChat.Core.Models.AppUser
{
    public class UserBasicFormViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }
}
