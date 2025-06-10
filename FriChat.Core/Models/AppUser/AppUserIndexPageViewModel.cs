namespace FriChat.Core.Models.AppUser
{
    public class AppUserIndexPageViewModel
    {
        public string ReturnUrl { get; set; } = string.Empty;
        public int UserId { get; set; }
        public IEnumerable<FriendsFormViewModed> FriendsList { get; set; } = new List<FriendsFormViewModed>();
        public string SearchTerm { get; set; }
        public IEnumerable<UserSearchFormViewModel> SearchResults { get; set; } = new List<UserSearchFormViewModel>();
    }
}
