using FluentValidation;
using ReservationSystem.Shared;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;
using UserService.API.Persistence.Entities;

namespace UserService.API.Features;

public record CreateUserRequest(string Username, string FirstName, string LastName, string Email);

public record CreateUserResponse(int Id);

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(20)
            .WithMessage("Username must be between 1 and 20 characters");
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(20)
            .WithMessage("First name must be between 1 and 20 characters");
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(20)
            .WithMessage("Last name must be between 1 and 20 characters");
        RuleFor(x => x.Email).NotEmpty().MaximumLength(50).EmailAddress()
            .WithMessage("Invalid email address");
    }
}

public class CreateUserHandler
{
    private readonly IUserService _userService;
    private readonly NetworkHttpClient _httpClient;

    public CreateUserHandler(IUserService userService, NetworkHttpClient httpClient)
    {
        _userService = userService;
        _httpClient = httpClient;
    }

    public async Task<ApiResult<int>> HandleAsync(CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            State = "Active",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };
        
        var id = await _userService.AddUserAsync(user, cancellationToken);

        if (id is null)
        {
            return new ApiResult<int>(0, false, "Failed to create user, duplicate email.");
        }
        

        return id is null
            ? new ApiResult<int>(0, false, "Failed to create user")
            : new ApiResult<int>(id.Value);
    }
}


public static class CreateUserEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/user",
            async (CreateUserRequest request,
                CreateUserHandler handler,
                CreateUserValidator validator,
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