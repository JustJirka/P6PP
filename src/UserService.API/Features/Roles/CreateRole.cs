using FluentValidation;
using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;
using UserService.API.Persistence.Entities;
using UserService.API.Services;

namespace UserService.API.Features.Roles;

public record CreateRoleRequest(string Name, string Description);

public class CreateRoleValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}

public class CreateRoleHandler
{
    private readonly IRoleService _roleService;

    public CreateRoleHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<ApiResult<int>> HandleAsync(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description
        };

        var id = await _roleService.AddRoleAsync(role, cancellationToken);

        return id.HasValue
            ? new ApiResult<int>(id.Value)
            : new ApiResult<int>(0, false, "Role not created");
    }
}


public static class CreateRoleEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/role",
            async (CreateRoleRequest request,
                CreateRoleHandler handler,
                CreateRoleValidator validator,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                
                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.HandleAsync(request, cancellationToken);
                
                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
    }
}