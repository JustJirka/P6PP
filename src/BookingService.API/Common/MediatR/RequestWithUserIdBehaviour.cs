using BookingService.API.Common.Exceptions;
using BookingService.API.Common.MediatR.Interfaces;
using MediatR;
using System.Security.Claims;

namespace BookingService.API.Common.MediatR;

public sealed class UserIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IRequestWithUserId
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserIdBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity
            ?? throw new AuthException("Invalid JWT: Claims Identity is null.");

        var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new AuthException("Invalid JWT: UserId claim is missing.");

        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new AuthException("Invalid JWT: UserId claim is not a valid integer.");
        }

        request.UserId = userId;

        return await next();
    }
}
