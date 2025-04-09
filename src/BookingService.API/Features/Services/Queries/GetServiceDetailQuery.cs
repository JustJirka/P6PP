using BookingService.API.Common.Exceptions;
using BookingService.API.Features.Services.Models;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.API.Features.Services.Queries;

public sealed class GetServiceDetailQuery : IRequest<ServiceResponse>
{
    public int ServiceId { get; set; }
    public GetServiceDetailQuery(int serviceId)
    {
        ServiceId = serviceId;
    }
}

public sealed class GetServiceDetailQueryHandler : IRequestHandler<GetServiceDetailQuery, ServiceResponse>
{
    private readonly DataContext _context;
    public GetServiceDetailQueryHandler(DataContext context)
    {
        _context = context;
    }
    public async Task<ServiceResponse> Handle(GetServiceDetailQuery request, CancellationToken cancellationToken)
    {
        var service = await _context.Services
            .Include(s => s.Room)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId, cancellationToken)
            ?? throw new NotFoundException("Service not found");
        return service.Map();
    }
}
