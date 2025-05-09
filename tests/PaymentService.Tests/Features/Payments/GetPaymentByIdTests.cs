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

public class GetPaymentByIdTests
{
    private readonly Mock<IPaymentRepository> _repositoryMock = new();
    private readonly PaymentService.API.Services.PaymentService _paymentService;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetPaymentByIdTests()
    {
        _paymentService = new PaymentService.API.Services.PaymentService(_repositoryMock.Object, new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()));
    }

    [Fact]
    public async Task GetPaymentByIdHandler_ShouldReturnPayment_WhenFound()
    {
        var handler = new GetPaymentByIdHandler(_paymentService);
        var request = new GetPaymentByIdRequest(1);
        var expected = new Payment { PaymentID = 1, UserId = 1, Price = 200 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, _token)).ReturnsAsync(expected);

        var result = await handler.HandleAsync(request, _token);

        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expected);
    }
}
