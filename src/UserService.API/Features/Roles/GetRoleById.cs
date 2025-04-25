using FluentValidation;
using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;
using UserService.API.Persistence.Entities;
using UserService.API.Services;

namespace UserService.API.Features.Roles;

public record GetRoleByIdRequest(int Id);

public class GetRoleByIdValidator : AbstractValidator<GetRoleByIdRequest>
{
    public GetRoleByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty()
            .GreaterThan(0).WithMessage("Invalid Id");
    }
}

public class GetRoleByIdHandler
{
    private readonly IRoleService _roleService;

    public GetRoleByIdHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<ApiResult<Role>> HandleAsync(GetRoleByIdRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var role = await _roleService.GetRoleByIdAsync(request.Id, cancellationToken);

        return role is null
            ? new ApiResult<Role>(null, false, "Role not found")
            : new ApiResult<Role>(role);
    }
}

public static class GetRoleByIdEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/role/{id:int}",
            async (int id,
                GetRoleByIdHandler handler,
                GetRoleByIdValidator validator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetRoleByIdRequest(id);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.HandleAsync(request, cancellationToken);

                return Results.Ok(result);
            });
    }
}