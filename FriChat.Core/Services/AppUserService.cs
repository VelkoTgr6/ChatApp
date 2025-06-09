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
        public async Task<IEnumerable<FriendsFormViewModed>> GetFriendsListAsync(int userId)
        {
            var user = await repository.AllAsReadOnly<AppUser>()
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);

            var userFriends= new List<FriendsFormViewModed>();

            if (user == null)
            {
                // Return an empty list to indicate no friends found or user does not exist
                return userFriends;
            }

            //return await repository.AllAsReadOnly<AppUser>()
            //    .Where(u => u.Id == userId && u.IsDeleted == false)
            //    .SelectMany(u => u.Friends)
            //    .Select(f => new FriendsFormViewModed
            //    {
            //        FriendUsername = f.Friends.Select(fr=>fr.UserName).ToString(),
            //        FriendUserId = f.Friends.Select(fr=>fr.Id).First(),
            //        FriendProfilePicturePath = f.Friends.Select(fr => fr.ProfilePicturePath).First() ?? string.Empty,
            //        LastMessage = f.Friends.Select(fr=>fr.Conversations.Select(c=>c.LastMessage)).ToString() ?? string.Empty
            //    })
            //    .ToListAsync();

            foreach (var friend in user.Friends)
            {
                var friendModel = new FriendsFormViewModed
                {
                    FriendUsername = friend.UserName,
                    FriendUserId = friend.Id,
                    FriendProfilePicturePath = friend.ProfilePicturePath,
                    LastMessage = friend.Friends.Select(c=>c.Conversations
                    .Where(c => c.UserId == userId && c.ReceiverUserId == friend.Id)
                            .OrderByDescending(c => c.CreatedAt)
                            .Select(c => c.LastMessage)
                            .FirstOrDefault())
                            .FirstOrDefault()
                            .ToString() ?? string.Empty
                };

                userFriends.Add(friendModel);
            }

            return userFriends;
        }

        public async Task<IEnumerable<UserSearchFormViewModel>> SearchUsersAsync(string searchTerm, int userId)
        {
            var users = new List<UserSearchFormViewModel>();
            var searchTermLower = searchTerm.ToLower();

            return await repository.AllAsReadOnly<AppUser>()
                .Where(u => (u.UserName.ToLower().Contains(searchTermLower) || u.FirstName.ToLower().Contains(searchTermLower) || 
                    u.LastName.ToLower().Contains(searchTermLower)) && u.Id != userId)
                .Select(u => new UserSearchFormViewModel
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProfilePictureUrl = u.ProfilePicturePath,
                    IsOnline = false
                })
                .ToListAsync();
        }

        //public async Task<int> GetUserIdAsync(string id)
        //{
        //    return await repository.GetUserIdByIdentityIdAsync(User.GetId().ToString());
        //}
    }
}
