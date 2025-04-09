using BookingService.API.Features.Services.Models;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.API.Features.Services.Queries;

public sealed class GetServicesQuery : IRequest<IList<ServiceResponse>>
{
    public int? TrainerId { get; set; }
    public GetServicesQuery(int trainedId)
    {
        TrainerId = trainedId;
    }
    public GetServicesQuery()
    {
    }
}

public sealed class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, IList<ServiceResponse>>
{
    private readonly DataContext _context;

    public GetServicesQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<IList<ServiceResponse>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Services.AsNoTracking();

        if (request.TrainerId.HasValue)
        {
            query = query.Where(s => s.TrainerId == request.TrainerId);
        }

        var services = await query.Include(s => s.Room).ToListAsync(cancellationToken);
        return services.Map();
    }
}