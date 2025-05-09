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

public class GetBalanceByIdTests
{
    private readonly Mock<IPaymentRepository> _repositoryMock = new();
    private readonly PaymentService.API.Services.PaymentService _paymentService;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetBalanceByIdTests()
    {
        _paymentService = new PaymentService.API.Services.PaymentService(_repositoryMock.Object, new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()));
    }

    [Fact]
    public async Task GetBalanceByIdHandler_ShouldReturnBalance_WhenFound()
    {
        var handler = new GetBalanceByIdHandler(_paymentService);
        var request = new GetBalanceByIdRequest(1);
        var expected = new UserCredit { UserId = 1, CreditBalance = 100 };
        _repositoryMock.Setup(r => r.GetBalanceByIdAsync(1, _token)).ReturnsAsync(expected);

        var result = await handler.HandleAsync(request, _token);

        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expected);
    }
}
