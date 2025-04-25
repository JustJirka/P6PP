using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;

namespace UserService.API.Features;

public record DeactivateUserRequest(int Id);

public class DeactivateUserValidator : AbstractValidator<DeactivateUserRequest>
{
    public DeactivateUserValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

    public class DeactivateUserHandler
{
    private readonly IUserService _userService;

    public DeactivateUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResult<bool>> HandleAsync(DeactivateUserRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userService.GetUserByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return new ApiResult<bool>(false, false, "User not found");
        }
        
        user.State = "Deactivated";
        
        var success = await _userService.UpdateUserAsync(user, cancellationToken);
        
        return success 
            ? new ApiResult<bool>(success) 
            : new ApiResult<bool>(success, false, "User not found");
    }
}


public static class DeactivateUserEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/user/deactivate/{id:int}",
                async (int id,
                    DeactivateUserHandler handler,
                    DeactivateUserValidator validator,
                    CancellationToken cancellationToken) =>
                {
                    var request = new DeactivateUserRequest(id);
                    var validationResult = await validator.ValidateAsync(request, cancellationToken);

                    if (!validationResult.IsValid)
                    {
                        var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                        return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages));
                    }

                    var result = await handler.HandleAsync(request, cancellationToken);

                    return result.Success
                        ? Results.Ok(result)
                        : Results.NotFound(result);
                })
            .WithName("DeactivateUser")
            .Produces<ApiResult<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResult<bool>>(StatusCodes.Status404NotFound);
    }
}