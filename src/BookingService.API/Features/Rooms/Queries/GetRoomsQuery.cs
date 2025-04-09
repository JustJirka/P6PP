using BookingService.API.Features.Rooms.Models;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.API.Features.Rooms.Queries;

public sealed class GetRoomsQuery : IRequest<IList<RoomResponse>> { }

public sealed class GetRoomsQueryHandler : IRequestHandler<GetRoomsQuery, IList<RoomResponse>>
{
    private readonly DataContext _context;

    public GetRoomsQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<IList<RoomResponse>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _context.Rooms
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rooms.Map();
    }
}
