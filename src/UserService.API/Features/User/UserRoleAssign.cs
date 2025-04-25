using FluentValidation;
using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;

namespace UserService.API.Features;

public record AssignUserRoleRequest(int UserId, int RoleId);

public class AssignUserRoleValidator : AbstractValidator<AssignUserRoleRequest>
{
    public AssignUserRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
        
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Role ID must be greater than 0");
    }
}

public class AssignUserRoleHandler
{
    private readonly IUserService _userService;

    public AssignUserRoleHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResult<bool>> HandleAsync(AssignUserRoleRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var success = await _userService.AssignUserRole(request.UserId, request.RoleId, cancellationToken);

        return success
            ? new ApiResult<bool>(success)
            : new ApiResult<bool>(success, false, "Role not assigned");
    }
}

public static class AssignUserRoleEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/user/role",
            async (AssignUserRoleRequest request,
                AssignUserRoleHandler handler,
                AssignUserRoleValidator validator,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult);
                }

                var result = await handler.HandleAsync(request, cancellationToken);
                
                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            });
    }
}