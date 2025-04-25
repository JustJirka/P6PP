namespace UserService.API.Abstraction
{
    using UserService.API.Persistence.Entities;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken);
        Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<int?> AddAsync(Role role, CancellationToken cancellationToken);
        Task UpdateAsync(Role role, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}