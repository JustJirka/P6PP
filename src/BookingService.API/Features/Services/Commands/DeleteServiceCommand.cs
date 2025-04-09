using BookingService.API.Common.Exceptions;
using BookingService.API.Domain.Enums;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.API.Features.Services.Commands;

public sealed class DeleteServiceCommand : IRequest
{
    public int ServiceId { get; set; }
    public DeleteServiceCommand(int serviceId)
    {
        ServiceId = serviceId;
    }
}

public sealed class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand>
{
    private readonly DataContext _context;
    public DeleteServiceCommandHandler(DataContext context)
    {
        _context = context;
    }
    public async Task Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(b => b.Id == request.ServiceId, cancellationToken)
            ?? throw new NotFoundException("Service not found");

        await _context.Bookings
              .Where(b => b.ServiceId == service.Id)
              .ExecuteUpdateAsync(b => b.SetProperty(b => b.Status, BookingStatus.Cancelled), cancellationToken);
        service.IsCancelled = true;
        _context.Services.Remove(service);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
