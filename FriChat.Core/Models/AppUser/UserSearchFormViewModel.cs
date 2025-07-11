﻿namespace FriChat.Core.Models.AppUser
{
    public class UserSearchFormViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public bool HasSearched { get; set; } = false;
        public bool IsFriend { get; set; } = false;
        public bool HasSentFriendRequest { get; set; } = false;
        public bool HasReceivedFriendRequest { get; set; } = false;
    }
}
