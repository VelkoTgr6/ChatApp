using Microsoft.AspNetCore.Identity;

namespace FriChat.Infrastructure.Data.Common
{
    public interface IRepository
    {
        Task<int> SaveChangesAsync();
        Task AddAsync<T>(T entity) where T : class;
        Task<IdentityUser?> GetIdentityUserByIdAsync(string id);
    }
}
