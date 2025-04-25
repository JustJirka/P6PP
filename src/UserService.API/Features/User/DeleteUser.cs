using FluentValidation;
using ReservationSystem.Shared;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;

namespace UserService.API.Features;

public record DeleteUserRequest(int Id);

public class DeleteUserValidator : AbstractValidator<DeleteUserRequest>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class DeleteUserHandler
{
    private readonly IUserService _userService;
    private readonly NetworkHttpClient _httpClient;

    public DeleteUserHandler(IUserService userService, NetworkHttpClient httpClient)
    {
        _userService = userService;
        _httpClient = httpClient;
    }

    public async Task<ApiResult<bool>> HandleAsync(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var success = await _userService.DeleteUserAsync(request.Id, cancellationToken);
        
        var authUrl = ServiceEndpoints.AuthService.DeleteUser(request.Id);
        var response = await _httpClient.DeleteAsync<object>(authUrl);
        
        if (!response.Success)
        {
            return new ApiResult<bool>(false, false, "Failed to delete user from AuthService");
        }
        
        return success 
            ? new ApiResult<bool>(success) 
            : new ApiResult<bool>(success, false, "User not found");
    }
}

public static class DeleteUserEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/user/{id:int}",
            async (int id,
                DeleteUserHandler handler,
                DeleteUserValidator validator,
                CancellationToken cancellationToken) =>
            {
                var request = new DeleteUserRequest(id);
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