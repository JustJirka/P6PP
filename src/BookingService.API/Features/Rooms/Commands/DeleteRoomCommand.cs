using BookingService.API.Common.Exceptions;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.API.Features.Rooms.Commands;

public sealed class DeleteRoomCommand : IRequest
{
    public DeleteRoomCommand(int roomId)
    {
        RoomId = roomId;
    }

    public int RoomId { get; set; }
}

public sealed class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand>
{
    private readonly DataContext _context;

    public DeleteRoomCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms
            .Include(r => r.Services)
            .SingleOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken)
            ?? throw new NotFoundException("Room not found");

        if (room.Services.Count != 0)
            throw new ValidationException("Room is used in services.");

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
