using FluentValidation;
using ReservationSystem.Shared.Results;
using UserService.API.Abstraction;
using UserService.API.Persistence.Entities;

namespace UserService.API.Features;

public record UpdateUserRequest(string Username, string FirstName, string LastName, string Email, string? PhoneNumber = null, decimal? Weight = null, decimal? Height = null, string? Sex = null);

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(20)
            .WithMessage("Username must be between 1 and 20 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(20)
            .WithMessage("First name must be between 1 and 20 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(20)
            .WithMessage("Last name must be between 1 and 20 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(50)
            .EmailAddress()
            .WithMessage("Invalid email address");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?\d{7,15}$") 
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Phone number must be between 7 and 15 digits and can start with '+'");

        RuleFor(x => x.Weight)
            .InclusiveBetween(30, 300) 
            .When(x => x.Weight.HasValue)
            .WithMessage("Weight must be between 30 and 300 kg");

        RuleFor(x => x.Height)
            .InclusiveBetween(100, 250) 
            .When(x => x.Height.HasValue)
            .WithMessage("Height must be between 100 and 250 cm");
        
        RuleFor(x => x.Sex)
            .Must(IsValidSex)
            .When(x => x.Sex != null)
            .WithMessage("Sex must be 'male' or 'female' or 'other'");
    }

    private bool IsValidSex(string sex)
    {
        sex = sex.ToLower();
        
        if (string.IsNullOrEmpty(sex)) return false;
        
        return string.Equals(sex, "male") || string.Equals(sex, "female") || string.Equals(sex, "other");
    }
}

public class UpdateUserHandler
{
    private readonly IUserService _userService;

    public UpdateUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResult<User>> HandleAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userService.GetUserByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return new ApiResult<User>(null, false, "User not found");
        }

        user.Username = request.Username;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
        user.Weight = request.Weight ?? user.Weight;
        user.Height = request.Height ?? user.Height;
        user.Sex = request.Sex ?? user.Sex;
        user.UpdatedOn = DateTime.UtcNow;

        await _userService.UpdateUserAsync(user, cancellationToken);

        return new ApiResult<User>(user, true, "User updated successfully");
    }
}

public static class UpdateUserEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/user/{id:int}",
            async (int id,
                UpdateUserRequest updateRequest,
                UpdateUserValidator validator,
                UpdateUserHandler handler,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(updateRequest, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.HandleAsync(id, updateRequest, cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            });
    }
}