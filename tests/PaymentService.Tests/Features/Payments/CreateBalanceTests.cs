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

public class CreateBalanceTests
{
    private readonly Mock<IPaymentRepository> _repositoryMock = new();
    private readonly PaymentService.API.Services.PaymentService _paymentService;
    private readonly CancellationToken _token = CancellationToken.None;

    public CreateBalanceTests()
    {
        _paymentService = new PaymentService.API.Services.PaymentService(_repositoryMock.Object, new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()));
    }

    [Fact]
    public async Task CreateBalanceHandler_ShouldReturnSuccessResult_WhenBalanceIsCreated()
    {
        var handler = new CreateBalanceHandler(_paymentService);
        var request = new CreateBalanceRequest(1);
        _repositoryMock.Setup(r => r.AddBalanceAsync(It.IsAny<UserCredit>(), _token)).ReturnsAsync(42);

        var result = await handler.HandleAsync(request, _token);

        result.Success.Should().BeTrue();
        result.Data.Should().Be(42);
    }
}
