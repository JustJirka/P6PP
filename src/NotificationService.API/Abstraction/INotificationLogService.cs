using src.NotificationService.API.Persistence.Entities.DB.Models;
using NotificationService.API.Persistence.Entities.DB;

namespace NotificationService.API.Services
{
    public interface INotificationLogService
    {
        Task LogNotification(int userId, NotificationType type, string subject, string text);
        Task<List<NotificationLog>> GetNotificationsFor(int userId, bool unreadOnly = true, int perPage = 20, int page = 0);
        Task<int> SetAllNotificationsAsRead(int userId);
        Task<int> SetSomeNotificationsAsRead(List<int> notificationIds);
    }
}
