using BookingService.API.Domain.Enums;

namespace BookingService.API.Features.Bookings.Models;

public record BookingResponse(
    int Id,
    int ServiceId,
    int UserId,
    BookingStatus Status);
