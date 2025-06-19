namespace FriChat.Core.Models.AppUser
{
    public class FriendsFormViewModed
    {
        public string FriendUsername { get; set; } = string.Empty;
        public int FriendUserId { get; set; }
        public string FriendProfilePicturePath { get; set; } = string.Empty;
        public string ? LastMessage { get; set; } = string.Empty;
        public bool IsOnline { get; set; } = false;
    }
}
