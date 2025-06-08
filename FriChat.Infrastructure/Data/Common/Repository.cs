using FriChat.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FriChat.Infrastructure.Data.Common
{
    public class Repository : IRepository
    {
        private readonly DbContext context;

        public Repository(FriChatDbContext _context)
        {
            context = _context;
        }
        private DbSet<T> DbSet<T>() where T : class
        {
            return context.Set<T>();
        }
        public async Task AddAsync<T>(T entity) where T : class
        {
            await DbSet<T>().AddAsync(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<IdentityUser?> GetIdentityUserByIdAsync(string id)
        {
            var user = await DbSet<IdentityUser>().FirstOrDefaultAsync(u => u.Id == id);
            return user; // Returns null if not found, caller can handle gracefully
        }

        public async Task<bool> EmailExistAsync(string email)
        {
            return await DbSet<AppUser>().AnyAsync(u => u.Email == email && u.IsDeleted == false);
        }

        public Task<bool> UsernameExistAsync(string username)
        {
            return DbSet<AppUser>().AnyAsync(u => u.UserName == username && u.IsDeleted == false);
        }
    }
}
