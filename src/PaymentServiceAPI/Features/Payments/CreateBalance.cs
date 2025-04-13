using FluentValidation;
using ReservationSystem.Shared.Results;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Services;


namespace PaymentService.API.Features.Payments;

public record CreateBalanceRequest(int UserId, int RoleId, int CreditBalance);

public class CreateBalanceValidator : AbstractValidator<CreateBalanceRequest>
{
    public CreateBalanceValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Neplatn� ID u�ivatele.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Neplatn� ID role.");

        RuleFor(x => x.CreditBalance)
            .GreaterThan(0).WithMessage("Zb�vaj�c� z�vazek mus� b�t v�t�� ne� 0.");

    }
}

public class CreateBalanceHandler
{
    private readonly PaymentService.API.Services.PaymentService _paymentService;

    public CreateBalanceHandler(PaymentService.API.Services.PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<ApiResult<int>> HandleAsync(CreateBalanceRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();


        var balance = new UserCredit
        {
            UserId = request.UserId,
            RoleId = request.RoleId,
            CreditBalance = request.CreditBalance,

        };
        var id = await _paymentService.CreateBalanceAsync(balance, cancellationToken);
        return id.HasValue
        ? new ApiResult<int>(id.Value)
        : new ApiResult<int>(0, false, "payment not created");


    }
}


public static class CreateBalanceEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/balance",
            async (CreateBalanceRequest request,
                CreateBalanceHandler handler,
                CreateBalanceValidator validator,
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