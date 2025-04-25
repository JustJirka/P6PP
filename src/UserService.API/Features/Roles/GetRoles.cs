using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;
using UserService.API.Persistence.Entities;
using UserService.API.Services;

namespace UserService.API.Features.Roles;

public class GetRolesHandler
{
    private readonly IRoleService _roleService;

    public GetRolesHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<ApiResult<IEnumerable<Role>>> HandleAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var roles = await _roleService.GetAllRolesAsync(cancellationToken);

        return roles.Any()
            ? new ApiResult<IEnumerable<Role>>(roles)
            : new ApiResult<IEnumerable<Role>>(Array.Empty<Role>(), false, "No roles found");
    }
}

public static class GetRolesEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/roles",
            async (GetRolesHandler handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.HandleAsync(cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            });
    }
}