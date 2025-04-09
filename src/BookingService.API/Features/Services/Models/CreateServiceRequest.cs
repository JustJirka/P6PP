namespace BookingService.API.Features.Services.Models;

public record CreateServiceRequest(
    DateTime Start,
    DateTime End,
    int Price,
    string ServiceName,
    int TrainerId,
    int RoomId);
