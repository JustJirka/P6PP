using System.ComponentModel.DataAnnotations;
using NotificationService.API.Persistence.Entities.DB.Models;

namespace src.NotificationService.API.Persistence.Entities.DB.Models;

public enum NotificationType {
    ReservationConfirmed,
    ReservationCanceled,
    EmailVerification,
    EmailRegistration,
    EmailPasswordReset,
    EmailOther,
}

public class NotificationLog : Entity
{
    public required int UserId { get; set; }
    public required NotificationType NotificationType { get; set; }
    [StringLength(75)]
    public required string Subject { get; set; }
    [StringLength(10000)]
    public required string Text { get; set; }
    [Required]
    public required DateTime SentDate { get; set; }
    [Required]
    public required bool HasBeeenRead { get; set; } = false;
}
