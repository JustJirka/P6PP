namespace BookingService.API.Features.Services.Models;

public record ServiceResponse(
    int Id,
    DateTime Start,
    DateTime End,
    int Price,
    string ServiceName,
    int CurrentCapacity,
    int TotalCapacity,
    string RoomName,
    bool IsCancelled);
