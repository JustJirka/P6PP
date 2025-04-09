using BookingService.API.Domain.Enums;

namespace BookingService.API.Features.Rooms.Models;

public record RoomRequest(
    string Name,
    int Capacity,
    RoomStatus Status
);
