namespace BookingService.API.Features.Services.Models;

public record UpdateServiceRequest(
    int Id,
    DateTime Start,
    DateTime End,
    string ServiceName,
    int RoomId);
