using Microsoft.EntityFrameworkCore;
using NotificationService.API.Persistence.Entities.DB.Models;
using src.NotificationService.API.Persistence.Entities.DB.Models;

namespace NotificationService.API.Abstraction
{
    public interface INotificationDbContext
    {
        DbSet<Booking> Bookings { get; }
        DbSet<NotificationLog> NotificationLogs { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
