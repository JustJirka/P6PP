
namespace NotificationService.API.Persistence.Entities;

public record BookingResponse(
    int id,
    int serviceId,
    string status,
    int userId);
