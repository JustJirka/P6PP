using System;
using Microsoft.EntityFrameworkCore;
using NotificationService.API.Persistence.Entities.DB;
using src.NotificationService.API.Persistence.Entities.DB.Models;

namespace src.NotificationService.API.Services;

public class NotificationLogService
{
    private readonly NotificationDbContext _notificationDbContext;

    public NotificationLogService(NotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
    }

    /// <summary>
    /// Creates new notification log
    /// </summary>
    public async Task LogNotification(int userId, NotificationType type, string subject, string text)
    {
        await _notificationDbContext.NotificationLogs.AddAsync(
            new NotificationLog
            {
                UserId = userId,
                NotificationType = type,
                Subject = subject,
                Text = text,
                SentDate = DateTime.Now,
                HasBeeenRead = false,
            }
        );
        await _notificationDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Get all notifications for userId.
    /// Notifications are ordered by Newest first
    /// </summary>
    /// <param name="userId">Return notifications for this UserID</param>
    /// <param name="unreadOnly">If is true (default), only unread notifications are returned</param>
    /// <param name="perPage">How many notifications to send on one page.
    /// When 0, returns all notifications starting from <paramref name="page"/>. Is 20 by default</param>
    /// <param name="page">Which page do you want returned. They are counted from 0</param>
    public async Task<List<NotificationLog>> GetNotificationsFor(int userId, bool unreadOnly = true, int perPage = 20, int page = 0)
    {
        var logs =  _notificationDbContext.NotificationLogs
            .Where(n => (n.UserId == userId));
        if (unreadOnly) {
            logs = logs.Where(n => n.HasBeeenRead == false);
        }

        logs = logs.OrderByDescending(n => n.SentDate)
                   .Skip(page * perPage);

        if (perPage > 0) {
            logs = logs.Take(perPage);
        }
        return await logs.ToListAsync();
    }

    public async Task<int> SetAllNotificationsAsRead(int userId)
    {
        var notificationsChanged = await _notificationDbContext
            .NotificationLogs
            .Where(n => n.UserId == userId)
            .ExecuteUpdateAsync(n => n.SetProperty(x => x.HasBeeenRead, true));

        await _notificationDbContext.SaveChangesAsync();
        return notificationsChanged;
    }

    public async Task<int> SetSomeNotificationsAsRead(List<int> notificationIds)
    {
        var notificationsChanged = await _notificationDbContext
            .NotificationLogs
            .Where(n => notificationIds.Contains(n.Id))
            .ExecuteUpdateAsync(n => n.SetProperty(x => x.HasBeeenRead, true));

        await _notificationDbContext.SaveChangesAsync();
        return notificationsChanged;
    }
}
