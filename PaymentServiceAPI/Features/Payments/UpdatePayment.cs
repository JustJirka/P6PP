
using FluentValidation;
using ReservationSystem.Shared.Results;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Services;


namespace PaymentService.API.Features.Payments;

public record UpdatePaymentRequest(int Id, string Status);

public class UpdatePaymentValidator : AbstractValidator<UpdatePaymentRequest>
{
    public UpdatePaymentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.")
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(status => status == "confirm" || status == "deny")
            .WithMessage("Status must be either 'confirm' or 'deny'.");
    }
}

public class UpdatePaymentHandler
{
    private readonly PaymentService.API.Services.PaymentService _paymentService;

    public UpdatePaymentHandler(PaymentService.API.Services.PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<ApiResult<int>> HandleAsync(UpdatePaymentRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request.Status == "deny")
        {
            var payment = new Payment
                {
                    Status = "failed",
                };

            var id = await _paymentService.ChangeStatus(payment, cancellationToken);

            return id.HasValue
                ? new ApiResult<int>(id.Value)
                : new ApiResult<int>(0, false, "payment not created");
        }

        var type = await _paymentService.GetTransactionType(request.Id, cancellationToken);


        if (type is null)
        {
            return new ApiResult<int>(0, false, "Transaction type not found");
        }

        if (type.Status == "completed")
        {
            return new ApiResult<int>(0, false, "Transaction already completed");
        }

        if (type.Status == "failed")
        {
            return new ApiResult<int>(0, false, "Transaction already failed");
        }



        if (type.TransactionType == "reservation")
        {
            var credits = await _paymentService.UpdateCreditsReservation(type.UserId, type.Price, cancellationToken);

            if (credits == "success")
            {

                var payment = new Payment
                {
                    Status = "completed",
                };

                var id = await _paymentService.ChangeStatus(payment, cancellationToken);

                return id.HasValue
                    ? new ApiResult<int>(id.Value)
                    : new ApiResult<int>(0, false, "payment not created");

            }
            else
            {
                var payment = new Payment
                {
                    Status = "failed",
                };
                var id = await _paymentService.ChangeStatus(payment, cancellationToken);
                return id.HasValue
                    ? new ApiResult<int>(id.Value)
                    : new ApiResult<int>(0, false, "payment not created");
            }
        }
        else
        {
            var credits = await _paymentService.UpdateCredits(type.UserId, type.CreditAmount, cancellationToken);

            if (credits == "success")
            {

                var payment = new Payment
                {
                    Status = "completed",
                };

                var id = await _paymentService.ChangeStatus(payment, cancellationToken);

                return id.HasValue
                    ? new ApiResult<int>(id.Value)
                    : new ApiResult<int>(0, false, "payment not created");

            }
            else
            {
                var payment = new Payment
                {
                    Status = "failed",
                };
                var id = await _paymentService.ChangeStatus(payment, cancellationToken);
                return id.HasValue
                    ? new ApiResult<int>(id.Value)
                    : new ApiResult<int>(0, false, "payment not created");
            }


        }


    }
}


public static class UpdatePaymentEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/payment{id}",
            async (UpdatePaymentRequest request,
                UpdatePaymentHandler handler,
                UpdatePaymentValidator validator,
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