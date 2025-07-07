using FriChat.Infrastructure.Services.CloudinaryServices;
using FriChat.Core.Common;
using FriChat.Core.Contracts;
using FriChat.Core.Models.AppUser;
using FriChat.Infrastructure.Data.Common;
using FriChat.Infrastructure.Data.Models;
using FriChat.Infrastructure.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FriChat.Core.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IRepository repository;
        private readonly ICloudinary cloudinary;

        public AppUserService(IRepository _repository, ICloudinary _cloudinary)
        {
            repository = _repository;
            cloudinary = _cloudinary;
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
                    (!u.Friends.Any(f => f.Id == userId) || !u.ReceivedFriendRequests.Any(f => f.Id == userId)));

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

        public async Task<ConversationFormViewModel> CreateConversationAsync(int userId, int friendId)
        {
            var friendData = await repository.AllAsReadOnly<AppUser>()
                .Where(u => u.Id == friendId && u.IsDeleted == false)
                .Select(u => new
                {
                    u.UserName,
                    u.ProfilePicturePath
                })
                .FirstOrDefaultAsync();

            var conversationEntity = new Conversation
            {
                UserId = userId,
                UserName = await repository.AllAsReadOnly<AppUser>()
                        .Where(u => u.Id == userId && u.IsDeleted == false)
                        .Select(u => u.UserName)
                        .FirstAsync(),
                ReceiverUserId = friendId,
                ReceiverUserName = friendData.UserName,
                IsGroupConversation = false,
                IsDeleted = false,
                ConversationName = friendData.UserName,
                ConversationImageUrl = friendData.ProfilePicturePath,
            };

            await repository.AddAsync(conversationEntity);
            await repository.SaveChangesAsync();

            var loadedConversation = await repository.AllAsReadOnly<Conversation>()
                .Include(c => c.User)
                .Include(c => c.ReceiverUser)
                .FirstOrDefaultAsync(c => c.Id == conversationEntity.Id);

            var conversationModel = new ConversationFormViewModel
            {
                ConversationId = conversationEntity.Id,
                ReceiverUserId = friendId,
                ReceiverUserName = conversationEntity.ReceiverUserName,
                ReceiverProfilePicturePath = loadedConversation.ReceiverUser.ProfilePicturePath,
                IsGroupConversation = conversationEntity.IsGroupConversation,
                UserProfilePicturePath = loadedConversation.User.ProfilePicturePath,
                ConversationName = conversationEntity.ReceiverUserName,
                ConversationPicturePath = conversationEntity.ConversationImageUrl
            };

            return conversationModel;
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

            user.ReceivedFriendRequests.Remove(friend);

            friend.FriendRequests.Remove(user);

            await repository.SaveChangesAsync();

            return userId;
        }

        public async Task<ConversationFormViewModel> GetConversationAsync(int userId, int friendId, int conversationId)
        {
            var newConversation = new ConversationFormViewModel();

            var conversation = await repository.AllAsReadOnly<Conversation>()
                .Where(u => u.Id == conversationId && u.IsDeleted == false)
                .Include(c => c.Messages)
                .Select(Conversation => new ConversationFormViewModel
                {
                    ReceiverUserId = friendId,
                    ReceiverUserName = Conversation.ReceiverUser.UserName,
                    ReceiverProfilePicturePath = Conversation.ReceiverUser.ProfilePicturePath,
                    IsGroupConversation = Conversation.IsGroupConversation,
                    ConversationId = Conversation.Id,
                    UserProfilePicturePath = Conversation.User.ProfilePicturePath,
                    ConversationName = Conversation.ReceiverUserName,
                    ConversationPicturePath = Conversation.ConversationImageUrl,
                }).FirstOrDefaultAsync();

            if (conversation == null)
            {
                // If the conversation does not exist, create a new one

                return await CreateConversationAsync(userId, friendId);
            }

            return conversation;
        }

        public Task<int> GetConversationIdAsync(int userId, int friendId)
        {
            return repository.AllAsReadOnly<Conversation>()
                .Include(c => c.User)
                .Include(c => c.ReceiverUser)
                .Where(c => ((c.UserId == userId && c.ReceiverUserId == friendId) ||
                             (c.UserId == friendId && c.ReceiverUserId == userId)) &&
                            !c.IsDeleted && !c.User.IsDeleted && !c.ReceiverUser.IsDeleted)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FriendsFormViewModed>> GetFriendsListAsync(int userId)
        {
            var user = await repository.AllAsReadOnly<AppUser>()
                .Include(u => u.Friends)
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);

            var userFriends = new List<FriendsFormViewModed>();

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

        public Task<int> GetUserFriendRequestsCount(int userId)
        {
            var count = repository.AllAsReadOnly<AppUser>()
                .Where(u => u.Id == userId && u.IsDeleted == false)
                .SelectMany(u => u.ReceivedFriendRequests)
                .CountAsync();

            return count;
        }

        public async Task<int> GetUserIdAsync(string identityId)
        {
            return await repository.AllAsReadOnly<AppUser>()
                .Where(s => s.IdentityUserId == identityId)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MessageViewModel>> GetUserMessagesForConversationAsync(int userId, int friendId, int conversationId)
        {
            var messages = await repository.AllAsReadOnly<Message>()
                .Include(m=>m.Receiver)
                .Include(m => m.Sender)
                .Include(m => m.UserMedia)
                .Where(m => ((m.SenderId == userId && m.ReceiverId == friendId) ||
                            (m.SenderId == friendId && m.ReceiverId == userId)) &&
                            m.ConversationId == conversationId)
                .OrderByDescending(m => m.Timestamp)
                .Take(50) // Limit to the last 50 messages
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            // Mark messages as read if they are from the friend and not already read
            if (messages.Any(m => m.SenderId == friendId && !m.IsRead))
            {
                foreach (var message in messages.Where(m => m.SenderId == friendId && !m.IsRead))
                {
                    message.IsRead = true;
                }
                await repository.SaveChangesAsync();
            }

            var decryptedMessages = messages.Select(m => new MessageViewModel
            {
                MessageId = m.Id,
                Content = EncryptionHelper.Decrypt(m.Content),
                Timestamp = m.Timestamp,
                IsRead = m.IsRead,
                SenderUserId = m.SenderId,
                ReceiverUserId = m.ReceiverId,
                AttachmentType = m.Type,
                AttachmentUrl = m.UserMedia.Url,
                UserId = userId,
                ConversationId = conversationId,
                UserProfilePicturePath = m.Sender.ProfilePicturePath,
                ReceiverProfilePicturePath = m.Receiver.ProfilePicturePath,
            });

            return decryptedMessages;
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

        public async Task<int> CreateMessageAsync(int userId, int friendId, IFormFile messageContent, MessageType messageType, int conversationId)
        {
            string encryptedContent = null;
            int? userMediaId = null;

            if (messageType == MessageType.Text || messageType == MessageType.Link)
            {
                // For text or link, just encrypt the content
                using var reader = new StreamReader(messageContent.OpenReadStream());
                var content = await reader.ReadToEndAsync();
                encryptedContent = EncryptionHelper.Encrypt(content);
            }
            else
            {
                // For media, upload and create UserMedia
                string? url = null;
                switch (messageType)
                {
                    case MessageType.Image:
                        url = await cloudinary.UploadImageFromUserAsync(messageContent, userId);
                        break;
                    case MessageType.Video:
                        url = await cloudinary.UploadVideoFromUserAsync(messageContent, userId);
                        break;
                    case MessageType.Audio:
                        url = await cloudinary.UploadAudioFromUserAsync(messageContent, userId);
                        break;
                    case MessageType.File:
                        url = await cloudinary.UploadFileFromUserAsync(messageContent, userId);
                        break;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    var userMedia = new UserMedia
                    {
                        Url = url,
                        Type = messageType,
                        UploadedAt = DateTime.UtcNow,
                        UserId = userId,
                        ConversationId = conversationId
                    };
                    await repository.AddAsync(userMedia);
                    await repository.SaveChangesAsync();
                    userMediaId = userMedia.Id;
                }
            }

            var message = new Message
            {
                SenderId = userId,
                ReceiverId = friendId,
                Content = encryptedContent ?? string.Empty,
                Timestamp = DateTime.UtcNow,
                IsRead = false,
                Type = messageType,
                ConversationId = conversationId,
                UserMediaId = userMediaId
            };

            await repository.AddAsync(message);
            await repository.SaveChangesAsync();

            return message.Id;
        }
    }
}
