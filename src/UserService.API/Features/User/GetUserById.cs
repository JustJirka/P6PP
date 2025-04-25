using FluentValidation;
using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;
using UserService.API.Persistence.Entities;

namespace UserService.API.Features;

public record GetUserByIdRequest(int Id);

public record GetUserByIdResponse(User User);

public class GetUserByIdValidator : AbstractValidator<GetUserByIdRequest>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0)
            .WithMessage("Invadil Id");
    }
}

public class GetUserByIdHandler
{
    private readonly IUserService _userService;
    
    public GetUserByIdHandler(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<ApiResult<GetUserByIdResponse>> HandleAsync(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var user = await _userService.GetUserByIdAsync(request.Id, cancellationToken);

        return user is null
            ? new ApiResult<GetUserByIdResponse>(null, false, "User not found")
            : new ApiResult<GetUserByIdResponse>(new GetUserByIdResponse(user));
    }
}

public static class GetUserByIdEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/user/{id:int}",
            async (int id,
                GetUserByIdHandler handler,
                GetUserByIdValidator validator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetUserByIdRequest(id);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                
                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.HandleAsync(request, cancellationToken);
                
                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            });
    }
}