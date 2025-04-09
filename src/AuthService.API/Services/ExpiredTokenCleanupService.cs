using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;

namespace AuthService.API.Services;

public class ExpiredTokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(2); 

    public ExpiredTokenCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

                var utcNow = DateTime.UtcNow;
                
                var expiredTokens = await dbContext.TokenBlackLists
                    .Where(t => t.ExpirationDate < utcNow)
                    .ToListAsync(stoppingToken);

                if (expiredTokens.Any())
                {
                    dbContext.TokenBlackLists.RemoveRange(expiredTokens);
                    await dbContext.SaveChangesAsync(stoppingToken);
                    Console.WriteLine($"Removed {expiredTokens.Count} expired tokens.");
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}