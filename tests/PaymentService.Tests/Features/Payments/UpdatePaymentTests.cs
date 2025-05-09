using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using PaymentService.API.Services;
using PaymentService.API.Features.Payments;
using PaymentService.API.Persistence.Entities.DB.Models;
using ReservationSystem.Shared.Results;
using PaymentService.API.Abstraction;

public class UpdatePaymentTests
{
    private readonly Mock<IPaymentRepository> _repositoryMock = new();
    private readonly PaymentService.API.Services.PaymentService _paymentService;
    private readonly CancellationToken _token = CancellationToken.None;

    public UpdatePaymentTests()
    {
        _paymentService = new PaymentService.API.Services.PaymentService(_repositoryMock.Object, new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()));
    }

    [Fact]
    public async Task UpdatePaymentHandler_ShouldCompleteReservation_WhenSufficientBalance()
    {
        var handler = new UpdatePaymentHandler(_paymentService);
        var request = new UpdatePaymentRequest(1, "confirm");
        var payment = new Payment
        {
            PaymentID = 1,
            UserId = 1,
            Price = 50,
            Status = "pending",
            TransactionType = "reservation"
        };

        var userCredit = new UserCredit { UserId = 1, CreditBalance = 100 };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, _token)).ReturnsAsync(payment);
        _repositoryMock.Setup(r => r.GetBalanceByIdAsync(1, _token)).ReturnsAsync(userCredit);
        _repositoryMock.Setup(r => r.UpdateCredits(It.IsAny<UserCredit>(), _token)).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.ChangeStatus(It.IsAny<Payment>(), _token)).ReturnsAsync(1);

        var result = await handler.HandleAsync(request, _token);

        result.Success.Should().BeTrue();
        result.Data.Should().Be(1);
    }
}
