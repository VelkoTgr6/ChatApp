using FriChat.Core.Models.AppUser;

namespace FriChat.Core.Contracts
{
    public interface IAppUserService
    {
        Task<IEnumerable<FriendsFormViewModed>> GetFriendsListAsync(int userId);
        Task<IEnumerable<UserSearchFormViewModel>> SearchUsersAsync(string searchTerm,int userId);
        Task<int> GetUserIdAsync(string identityId);
        Task<int> AddFriendToUserAsync(int userId, int friendId);

        //Task<int> GetUserIdAsync(string id);
    }
}
