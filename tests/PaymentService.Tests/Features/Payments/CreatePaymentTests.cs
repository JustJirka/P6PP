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

public class CreatePaymentTests
{
    private readonly Mock<IPaymentRepository> _repositoryMock = new();
    private readonly PaymentService.API.Services.PaymentService _paymentService;
    private readonly CancellationToken _token = CancellationToken.None;

    public CreatePaymentTests()
    {
        _paymentService = new PaymentService.API.Services.PaymentService(_repositoryMock.Object, new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()));
    }

    [Fact]
    public async Task CreatePaymentHandler_ShouldCreateCreditPayment_WhenTransactionTypeIsCredit()
    {
        var handler = new CreatePaymentHandler(_paymentService);
        var request = new CreatePaymentRequest(1, 1, "credit", 100);
        _repositoryMock.Setup(r => r.AddAsyncCredits(It.IsAny<Payment>(), _token)).ReturnsAsync(77);

        var result = await handler.HandleAsync(request, _token);

        result.Success.Should().BeTrue();
        result.Data.Should().Be(77);
    }
}
