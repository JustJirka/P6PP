using Microsoft.Extensions.Caching.Memory;
using PaymentService.API.Persistence.Entities;
using PaymentService.API.Persistence.Repositories;

namespace PaymentService.API.Services;

public class CreditService
{
    private readonly CreditRepository _creditRepository;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "user";
    
    public CreditService(CreditRepository userRepository, IMemoryCache cache)
    {
        _creditRepository = userRepository;
        _cache = cache;
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
            
            var newUserId = await _creditRepository.AddAsync(user, cancellationToken);
            
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
        var success = await _creditRepository.UpdateAsync(user, cancellationToken);

        if (success)
        {
            _cache.Remove(cacheKey);
        }
        
        return success;
    }
    
    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken)
    {
        string cacheKey = $"{CacheKey}:{id}";
        var success = await _creditRepository.DeleteAsync(id, cancellationToken);

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

        var user = await _creditRepository.GetByIdAsync(userId, cancellationToken);
        
        if (user is null)
        {
            return false;
        }
        
        user.RoleId = roleId;
        return await _creditRepository.UpdateAsync(user, cancellationToken);
    }
}