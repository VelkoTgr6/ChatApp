using Microsoft.AspNetCore.Identity;

namespace FriChat.Infrastructure.Data.Common
{
    public interface IRepository
    {
        Task<int> SaveChangesAsync();
        Task AddAsync<T>(T entity) where T : class;
        Task<IdentityUser?> GetIdentityUserByIdAsync(string id);
        Task<bool> EmailExistAsync(string email);
        Task<bool> UsernameExistAsync(string username);
        Task<int> GetUserIdByIdentityIdAsync(string id);
        IQueryable<T> AllAsReadOnly<T>() where T : class;
        IQueryable<T> All<T>() where T : class;
    }
}
