using BookingService.API.Common.Exceptions;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BookingService.API.Features.Bookings.Models;

namespace BookingService.API.Features.Bookings.Queries;

public sealed class GetBookingDetailQuery : IRequest<BookingResponse>
{
    public GetBookingDetailQuery(int bookingId)
    {
        BookingId = bookingId;
    }

    public int BookingId { get; set; }
}

public sealed class GetBookingDetailQueryHandler : IRequestHandler<GetBookingDetailQuery, BookingResponse>
{
    private readonly DataContext _context;

    public GetBookingDetailQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<BookingResponse> Handle(GetBookingDetailQuery request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken)
                ?? throw new NotFoundException("Booking not found");

        return booking.Map();
    }
}