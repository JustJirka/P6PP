using FluentValidation;
using ReservationSystem.Shared.Results;



namespace PaymentService.API.Features.Payments;

public record CreatePaymentRequest(string UserId, string Price);

public class CreatePaymentValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Price).NotEmpty();
    }
}

public class CreatePaymentHandler
{
    private readonly PaymentService _paymentService;

    public CreatePaymentHandler(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<ApiResult<int>> HandleAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var role = new Payment;
        {
            Name = request.UserId,                            
            Description = request.Price
        };

        var id = await _paymentService.AddRoleAsync(role, cancellationToken);

        return id.HasValue
            ? new ApiResult<int>(id.Value)
            : new ApiResult<int>(0, false, "Role not created");
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