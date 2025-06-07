namespace FriChat.Infrastructure.Data.Common
{
    public interface IRepository
    {
        Task<int> SaveChangesAsync();
        Task AddAsync<T>(T entity) where T : class;
    }
}
