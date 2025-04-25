using UserService.API.Persistence.Entities;

namespace UserService.API.Abstraction;

public interface IUserService
{
    Task<(IEnumerable<User> Users, int TotalCount)> GetAllUsersAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<int?> AddUserAsync(User user, CancellationToken cancellationToken);
    Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken);
    Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken);
    Task<bool> AssignUserRole(int userId, int roleId, CancellationToken cancellationToken);
}