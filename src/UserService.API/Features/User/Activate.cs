using FluentValidation;
using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;

namespace UserService.API.Features;

public record ActivateUserRequest(int Id);

public class ActivateUserValidator : AbstractValidator<ActivateUserRequest>
{
    public ActivateUserValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class ActivateUserHandler
{
    private readonly IUserService _userService;

    public ActivateUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResult<bool>> HandleAsync(ActivateUserRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userService.GetUserByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return new ApiResult<bool>(false, false, "User not found");
        }
        
        user.State = "Active";
        
        var success = await _userService.UpdateUserAsync(user, cancellationToken);
        
        return success 
            ? new ApiResult<bool>(success) 
            : new ApiResult<bool>(success, false, "User not found");
    }
}

public static class ActivateUserEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/user/activate/{id:int}",
            async (int id,
                ActivateUserHandler handler,
                ActivateUserValidator validator,
                CancellationToken cancellationToken) =>
            {
                var request = new ActivateUserRequest(id);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    return Results.BadRequest(
                        new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.HandleAsync(request, cancellationToken);
                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            });
    }
}