
using FluentValidation;
using ReservationSystem.Shared.Results;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Services;


namespace PaymentService.API.Features.Payments;

public record CreateStatusRequest(string Status);

public class CreateStatusValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreateStatusValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Neplatné ID uživatele.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Částka musí být větší než 0.");
    }
}

public class CreateStatusHandler
{
    private readonly PaymentService.API.Services.PaymentService _paymentService;

    public CreateStatusHandler(PaymentService.API.Services.PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<ApiResult<int>> HandleAsync(CreateStatusRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payment = new Payment
        {
            Status = request.Status,
        };

        var id = await _paymentService.ChangeStatus(payment, cancellationToken);

        return id.HasValue
            ? new ApiResult<int>(id.Value)
            : new ApiResult<int>(0, false, "payment not created");
    }
}


public static class CreatePaymentEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/role",
            async (CreatePaymentRequest request,
                CreatePaymentHandler handler,
                CreatePaymentValidator validator,
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