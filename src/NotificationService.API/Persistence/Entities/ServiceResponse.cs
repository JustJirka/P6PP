namespace NotificationService.API.Persistence.Entities;

public record ServiceResponse(
    int id,
    DateTime start,
    DateTime end,
    int price,
    string serviceName,
    int currentCapacity,
    int totalCapacity,
    string roomName,
    bool isCancelled);
