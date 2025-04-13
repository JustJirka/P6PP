using FluentValidation;
using ReservationSystem.Shared.Results;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Services;

namespace PaymentService.API.Features.Payments;

public record GetPaymentByIdRequest(int Id);

public class GetPaymentByIdValidator : AbstractValidator<GetPaymentByIdRequest>
{
    public GetPaymentByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty()
            .GreaterThan(0).WithMessage("Invalid Id");
    }
}

public class GetPaymentByIdHandler
{
    private readonly PaymentService.API.Services.PaymentService _PaymentService;

    public GetPaymentByIdHandler(PaymentService.API.Services.PaymentService PaymentService)
    {
        _PaymentService = PaymentService;
    }

    public async Task<ApiResult<Payment>> HandleAsync(GetPaymentByIdRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payment = await _PaymentService.GetPaymentByIdAsync(request.Id, cancellationToken);

        return payment is null
            ? new ApiResult<Payment>(null, false, "Role not found")
            : new ApiResult<Payment>(payment);
    }
}

public static class GetPaymentByIdEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payment/{id:int}",
            async (int id,
                GetPaymentByIdHandler handler,
                GetPaymentByIdValidator validator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetPaymentByIdRequest(id);
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