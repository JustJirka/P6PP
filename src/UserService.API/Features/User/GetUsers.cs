using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;
using UserService.API.Persistence.Entities;

namespace UserService.API.Features;

public record GetUsersResponse(IEnumerable<User> Users, int TotalCount);

public class GetUsersHandler
{
    private readonly IUserService _userService;
    
    public GetUsersHandler(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<ApiResult<GetUsersResponse>> HandleAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var (users, totalCount) = await _userService.GetAllUsersAsync(page, pageSize, cancellationToken);

        return users.Any() is false
            ? new ApiResult<GetUsersResponse>(null, false, "Users not found")
            : new ApiResult<GetUsersResponse>(new GetUsersResponse(users, totalCount));
    }
}

public static class GetUsersEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users",
            async (GetUsersHandler handler,
                CancellationToken cancellationToken,
                int page = 1,
                int pageSize = 10) =>
            {
                var result = await handler.HandleAsync(page, pageSize, cancellationToken);
                
                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            });
    }
}