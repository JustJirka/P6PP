using BookingService.API.Domain.Models;

namespace BookingService.API.Features.Bookings.Models;

public static class BookingMappingExtensions
{
    public static Booking Map(this CreateBookingRequest input)
        => new()
        {
            ServiceId = input.ServiceId
        };

    public static BookingResponse Map(this Booking input)
        => new(input.Id, input.ServiceId, input.Status);

    public static IList<BookingResponse> Map(this IList<Booking> input)
        => [.. input.Select(Map)];
}
