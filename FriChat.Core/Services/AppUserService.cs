using FriChat.Core.Contracts;
using FriChat.Core.Models.AppUser;
using FriChat.Infrastructure.Data.Common;
using FriChat.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FriChat.Core.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IRepository repository;

        public AppUserService(IRepository _repository)
        {
            repository = _repository;
        }

        public async Task<int> AddFriendRequestToUserAsync(int userId, int friendId)
        {
            var user = await repository.All<AppUser>()
                .Include(u => u.FriendRequests)
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);

            var friend = await repository.All<AppUser>()
                .Include(u => u.ReceivedFriendRequests)
                .Include(u => u.Friends)
                .FirstOrDefaultAsync(u => u.Id == friendId && u.IsDeleted == false && 
                    (!u.Friends.Any(f => f.Id == userId) || !u.ReceivedFriendRequests.Any(f=>f.Id == userId)));

            if (user == null || friend == null)
            {
                // Return -1 to indicate that either user or friend does not exist
                return -1;
            }

            var model = new AddFriendFormModel
            {
                UserId = userId,
                FriendId = friendId
            };

            user.FriendRequests.Add(friend);

            friend.ReceivedFriendRequests.Add(user);

            await repository.SaveChangesAsync();

            return userId;
        }

        public async Task<int> ConfirmFriendRequestAsync(int userId, int friendId)
        {
            var user = await repository.All<AppUser>()
                .Include(u => u.Friends)
                .Include(u => u.ReceivedFriendRequests)
               .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);

            var friend = await repository.All<AppUser>()
                .Include(u => u.Friends)
                .Include(u => u.FriendRequests)
                .FirstOrDefaultAsync(u => u.Id == friendId && u.IsDeleted == false &&
                    (!u.Friends.Any(f => f.Id == userId)));

            if (user == null || friend == null)
            {
                // Return -1 to indicate that either user or friend does not exist
                return -1;
            }

            var model = new AddFriendFormModel
            {
                UserId = userId,
                FriendId = friendId
            };

            user.Friends.Add(friend);
            user.ReceivedFriendRequests.Remove(friend);

            friend.Friends.Add(user);
            friend.FriendRequests.Remove(user);

            await repository.SaveChangesAsync();

            return userId;
        }

        public async Task<int> DeclineFriendRequestAsync(int userId, int friendId)
        {
            var user = await repository.All<AppUser>()
                .Include(u => u.ReceivedFriendRequests)
               .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);

            var friend = await repository.All<AppUser>()
                .Include(u => u.FriendRequests)
                .FirstOrDefaultAsync(u => u.Id == friendId && u.IsDeleted == false &&
                    (!u.Friends.Any(f => f.Id == userId)));

            if (user == null || friend == null)
            {
                // Return -1 to indicate that either user or friend does not exist
                return -1;
            }

            var model = new AddFriendFormModel
            {
                UserId = userId,
                FriendId = friendId
            };

            user.ReceivedFriendRequests.Remove(friend);

            friend.FriendRequests.Remove(user);

            await repository.SaveChangesAsync();

            return userId;
        }

        public async Task<IEnumerable<FriendsFormViewModed>> GetFriendsListAsync(int userId)
        {
            var user = await repository.AllAsReadOnly<AppUser>()
                .Include(u => u.Friends)
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);

            var userFriends= new List<FriendsFormViewModed>();

            if (user == null)
            {
                // Return an empty list to indicate no friends found or user does not exist
                return userFriends;
            }

            userFriends = await repository.AllAsReadOnly<AppUser>()
                .Where(u => u.Id == userId && u.IsDeleted == false)
                .Include(u => u.Friends)
                .SelectMany(u => u.Friends)
                .Select(f => new FriendsFormViewModed
                {
                    FriendUserId = f.Id,
                    FriendUsername = f.UserName,
                    FriendProfilePicturePath = f.ProfilePicturePath,
                    LastMessage = f.Conversations
                   .Select(c => c.LastMessage).ToString() ?? string.Empty,
                    IsOnline = false
                })
                .ToListAsync();

            return userFriends;
        }

        public async Task<IEnumerable<UserBasicFormViewModel>> GetUserFriendRequestsAsync(int userId)
        {
            var friendRequests = await repository.AllAsReadOnly<AppUser>()
                .Include(u => u.ReceivedFriendRequests)
                .Where(u => u.Id == userId && u.IsDeleted == false)
                .SelectMany(u => u.ReceivedFriendRequests)
                .Select(f => new UserBasicFormViewModel
                {
                    UserId = f.Id,
                    Username = f.UserName,
                    FirstName = f.FirstName,
                    LastName = f.LastName,
                    ProfilePictureUrl = f.ProfilePicturePath
                })
                .ToListAsync();

            return friendRequests;
        }

        public async Task<int> GetUserIdAsync(string identityId)
        {
            return await repository.AllAsReadOnly<AppUser>()
                .Where(s => s.IdentityUserId == identityId)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserSearchFormViewModel>> SearchUsersAsync(string searchTerm, int userId)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || userId <= 0)
            {
                return new List<UserSearchFormViewModel>();
            }

            var searchTermLower = searchTerm.ToLower();

            var users = await repository.AllAsReadOnly<AppUser>()
                .Include(u => u.Friends)
                .Include(u => u.ReceivedFriendRequests)
                .Include(u => u.FriendRequests)
                .Where(u => u.IsDeleted == false)
                .Where(u => (u.UserName.ToLower().Contains(searchTermLower) || u.FirstName.ToLower().Contains(searchTermLower) || 
                    u.LastName.ToLower().Contains(searchTermLower)) && u.Id != userId)
                .Select(u => new UserSearchFormViewModel
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProfilePictureUrl = u.ProfilePicturePath,
                    IsOnline = false,
                    SearchTerm = searchTerm,
                    HasReceivedFriendRequest = u.ReceivedFriendRequests.Any(f => f.Id == userId),
                    HasSentFriendRequest = u.FriendRequests.Any(f => f.Id == userId),
                    IsFriend = u.Friends.Any(f => f.Id == userId)
                })
                .ToListAsync();

            return users;
        }

    }
}
