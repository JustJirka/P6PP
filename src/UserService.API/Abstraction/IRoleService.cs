using UserService.API.Persistence.Entities;

namespace UserService.API.Abstraction;

public interface IRoleService
{
    Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken);
    Task<Role?> GetRoleByIdAsync(int id, CancellationToken cancellationToken);
    Task<int?> AddRoleAsync(Role role, CancellationToken cancellationToken);
}