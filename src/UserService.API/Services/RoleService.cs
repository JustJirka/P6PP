using Microsoft.Extensions.Caching.Memory;
using UserService.API.Abstraction;
using UserService.API.Persistence.Entities;

namespace UserService.API.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMemoryCache _cache;
    
    public RoleService(IRoleRepository roleRepository, IMemoryCache cache)
    {
        _roleRepository = roleRepository;
        _cache = cache;
    }
    
    public async Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken)
    {
        return await _roleRepository.GetAllAsync(cancellationToken);
    }
    
    public async Task<Role?> GetRoleByIdAsync(int id, CancellationToken cancellationToken)
    {
        string cacheKey = $"role:{id}";

        if (!_cache.TryGetValue(cacheKey, out Role? role))
        {
            role = await _roleRepository.GetByIdAsync(id, cancellationToken);

            if (role != null)
            {
                _cache.Set(cacheKey, role, TimeSpan.FromMinutes(10));
            }
        }

        return role;
    }
    
    public async Task<int?> AddRoleAsync(Role role, CancellationToken cancellationToken)
    {
        string cacheKey = $"role:{role.Id}";
        var newRole = await _roleRepository.AddAsync(role, cancellationToken);
        
        if (newRole != null)
        {
            _cache.Set(cacheKey, role, TimeSpan.FromMinutes(10));
        }
        
        return newRole;
    }
}