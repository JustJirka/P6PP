using BookingService.API.Domain.Enums;

namespace BookingService.API.Features.Rooms.Models;

public record RoomResponse(
    int Id,
    string Name,
    int Capacity,
    RoomStatus Status
);
