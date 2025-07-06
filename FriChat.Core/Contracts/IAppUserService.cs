using FriChat.Core.Models.AppUser;

namespace FriChat.Core.Contracts
{
    public interface IAppUserService
    {
        Task<IEnumerable<FriendsFormViewModed>> GetFriendsListAsync(int userId);
        Task<IEnumerable<UserSearchFormViewModel>> SearchUsersAsync(string searchTerm,int userId);
        Task<int> GetUserIdAsync(string identityId);
        Task<int> AddFriendRequestToUserAsync(int userId, int friendId);
        Task<IEnumerable<UserBasicFormViewModel>> GetUserFriendRequestsAsync(int userId);
        Task<int> ConfirmFriendRequestAsync(int userId, int friendId);
        Task<int> DeclineFriendRequestAsync(int userId, int friendId);
        Task<int> GetUserFriendRequestsCount(int userId);
        Task<IEnumerable<MessageViewModel>> GetUserMessagesForConversationAsync(int userId, int friendId,int conversationId);
        Task<ConversationFormViewModel> GetConversationAsync(int userId, int friendId,int coversationId);
        Task<ConversationFormViewModel> CreateConversationAsync(int userId, int friendId);
        Task<int> GetConversationIdAsync(int userId, int friendId);
        Task <int> CreateMessageAsync(int userId, int friendId, string messageContent, int conversationId);
    }
}
