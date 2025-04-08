using FluentValidation;
using ReservationSystem.Shared.Results;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Services;

namespace PaymentService.API.Features.Payments;

public record GetBalanceByIdRequest(int UserId);

public class GetBalanceByIdValidator : AbstractValidator<GetBalanceByIdRequest>
{
    public GetBalanceByIdValidator()
    {
        RuleFor(x => x.UserId).NotEmpty()
            .GreaterThan(0).WithMessage("Invalid Id");
    }
}

public class GetBalanceByIdHandler
{
    private readonly PaymentService.API.Services.PaymentService _PaymentService;

    public GetBalanceByIdHandler(PaymentService.API.Services.PaymentService PaymentService)
    {
        _PaymentService = PaymentService;
    }

    public async Task<ApiResult<UserCredit>> HandleAsync(GetBalanceByIdRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payment = await _PaymentService.GetBalanceByIdAsync(request.UserId, cancellationToken);

        return payment is null
            ? new ApiResult<UserCredit>(null, false, "balance not found")
            : new ApiResult<UserCredit>(payment);
    }
}

public static class GetBalanceByIdEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/credit/{UserId:int}",
            async (int UserId,
                GetBalanceByIdHandler handler,
                GetBalanceByIdValidator validator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetBalanceByIdRequest(UserId);
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