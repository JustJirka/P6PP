using BookingService.API.Common.Exceptions;
using BookingService.API.Features.Rooms.Models;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.API.Features.Rooms.Commands;

public sealed class UpdateRoomCommand : IRequest<RoomResponse>
{
    public UpdateRoomCommand(int roomId, RoomRequest room)
    {
        RoomId = roomId;
        Room = room;
    }

    public int RoomId { get; set; }
    public RoomRequest Room { get; set; }
}

public sealed class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, RoomResponse>
{
    private readonly DataContext _context;

    public UpdateRoomCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<RoomResponse> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms
            .SingleOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken)
            ?? throw new NotFoundException("Room not found");

        request.Room.MapTo(room);
        await _context.SaveChangesAsync(cancellationToken);

        return room.Map();
    }
}
