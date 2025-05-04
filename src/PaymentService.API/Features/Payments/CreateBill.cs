using FluentValidation;
using ReservationSystem.Shared.Results;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Services;

namespace PaymentService.API.Features.Payments;

public record CreateBillRequest(int Id);

public class CreateBillValidator : AbstractValidator<CreateBillRequest>
{
	public CreateBillValidator()
	{
		RuleFor(x => x.Id).NotEmpty()
			.GreaterThan(0).WithMessage("Invalid Id");
	}
}

public class CreateBillHandler
{
	private readonly PaymentService.API.Services.PaymentService _PaymentService;

	public CreateBillHandler(PaymentService.API.Services.PaymentService PaymentService)
	{
		_PaymentService = PaymentService;
	}

	public async Task<ApiResult<Payment>> HandleAsync(CreateBillRequest request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var payment = await _PaymentService.CreateBillAsync(request.Id, cancellationToken);

		return payment != "Success"
			? new ApiResult<Payment>(null, false, "Payments not found")
			: new ApiResult<Payment>(payment);
	}
}

public static class CreateBillEndpoint
{
	public static void Register(IEndpointRouteBuilder app)
	{
		app.MapGet("/api/createbill/{id:int}",
			async (int id,
				CreateBillHandler handler,
				CreateBillValidator validator,
				CancellationToken cancellationToken) =>
			{
				var request = new CreateBillRequest(id);
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