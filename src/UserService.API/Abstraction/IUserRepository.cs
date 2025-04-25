using UserService.API.Persistence.Entities;

namespace UserService.API.Abstraction
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalUserCountAsync(CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<int?> AddAsync(User user, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}