using Microsoft.Extensions.Caching.Memory;
using UserService.API.Abstraction;
using UserService.API.Exceptions;
using UserService.API.Persistence.Entities;

namespace UserService.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "user";
    
    public UserService(IUserRepository userRepository, IMemoryCache cache, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _cache = cache;
        _roleRepository = roleRepository;
    }
    
    public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllUsersAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        var users = await _userRepository.GetAllAsync(page, pageSize, cancellationToken);
        var totalCount = await _userRepository.GetTotalUserCountAsync(cancellationToken);
        
        foreach (var user in users)
        {
            user.Role = roles.FirstOrDefault(x => x.Id == user.RoleId);
        }
        
        return (users, totalCount);
    }
    
    public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        string cacheKey = $"{CacheKey}:{id}";

        if (!_cache.TryGetValue(cacheKey, out User? user))
        {
            user = await _userRepository.GetByIdAsync(id, cancellationToken);

            if (user != null)
            {
                user.Role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(10));
            }
        }

        return user;
    }
    
    public async Task<int?> AddUserAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
    
        string cacheKey = $"{CacheKey}:{user.Email}";

        try
        {
            var allRoles = await _roleRepository.GetAllAsync(cancellationToken);
            var role = allRoles.FirstOrDefault(x => x.Name == "User");
            user.RoleId = role.Id;
            
            var newUserId = await _userRepository.AddAsync(user, cancellationToken);
            
            if (newUserId != null)
            {
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(10));
            }

            return newUserId;
        }
        catch (DuplicateEntryException)
        {
            return null;
        }
    }
    
    public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        string cacheKey = $"{CacheKey}:{user.Id}";
        var success = await _userRepository.UpdateAsync(user, cancellationToken);

        if (success)
        {
            _cache.Remove(cacheKey);
        }
        
        return success;
    }
    
    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken)
    {
        string cacheKey = $"{CacheKey}:{id}";
        var success = await _userRepository.DeleteAsync(id, cancellationToken);

        if (success)
        {
            _cache.Remove(cacheKey);
        }
        
        return success;
    }

    public async Task<bool> AssignUserRole(int userId, int roleId, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        
        if (role is null)
        {
            return false;
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        
        if (user is null)
        {
            return false;
        }
        
        user.RoleId = roleId;
        return await _userRepository.UpdateAsync(user, cancellationToken);
    }
}