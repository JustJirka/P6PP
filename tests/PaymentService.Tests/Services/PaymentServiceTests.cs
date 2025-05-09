using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PaymentService.API.Services;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Abstraction;
using FluentAssertions;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _repositoryMock;
    private readonly IMemoryCache _cache;
    private readonly PaymentService.API.Services.PaymentService _service;

    public PaymentServiceTests()
    {
        _repositoryMock = new Mock<IPaymentRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _service = new PaymentService.API.Services.PaymentService(_repositoryMock.Object, _cache);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_ReturnsPayment()
    {
        var payment = new Payment { PaymentID = 1, UserId = 1, Price = 100, TransactionType = "reservation" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

        var result = await _service.GetPaymentByIdAsync(1, CancellationToken.None);

        result.Should().Be(payment);
    }

    [Fact]
    public async Task CreatePayment_ReturnsNewId()
    {
        var payment = new Payment { UserId = 1, RoleId = 1, Price = 100, Status = "pending" };
        _repositoryMock.Setup(r => r.AddAsync(payment, It.IsAny<CancellationToken>())).ReturnsAsync(123);

        var result = await _service.CreatePayment(payment, CancellationToken.None);

        result.Should().Be(123);
    }

    [Fact]
    public async Task UpdateCreditsReservation_ReturnsSuccess_WhenSufficientBalance()
    {
        var userCredit = new UserCredit { UserId = 1, CreditBalance = 200 };
        _repositoryMock.Setup(r => r.GetBalanceByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userCredit);
        _repositoryMock.Setup(r => r.UpdateCredits(It.IsAny<UserCredit>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateCreditsReservation(1, 100, CancellationToken.None);

        result.Should().Be("Success");
    }

    [Fact]
    public async Task UpdateCreditsReservation_ReturnsFailed_WhenInsufficientBalance()
    {
        var userCredit = new UserCredit { UserId = 1, CreditBalance = 50 };
        _repositoryMock.Setup(r => r.GetBalanceByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userCredit);

        var result = await _service.UpdateCreditsReservation(1, 100, CancellationToken.None);

        result.Should().Be("Failed");
    }

    [Fact]
    public async Task CreatePaymentCredits_ReturnsNewId()
    {
        var payment = new Payment { UserId = 1, RoleId = 1, CreditAmount = 100, Status = "pending" };
        _repositoryMock.Setup(r => r.AddAsyncCredits(payment, It.IsAny<CancellationToken>())).ReturnsAsync(321);

        var result = await _service.CreatePaymentCredits(payment, CancellationToken.None);

        result.Should().Be(321);
    }

    [Fact]
    public async Task ChangeStatus_UpdatesStatusSuccessfully()
    {
        var payment = new Payment { PaymentID = 5, Status = "completed" };
        _repositoryMock.Setup(r => r.ChangeStatus(payment, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _service.ChangeStatus(payment, CancellationToken.None);

        result.Should().Be(1);
    }

    [Fact]
    public async Task GetTransactionType_ReturnsNull_WhenInvalidType()
    {
        var payment = new Payment { PaymentID = 1, TransactionType = "invalid" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

        var result = await _service.GetTransactionType(1, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTransactionType_ReturnsPayment_WhenTypeIsCredit()
    {
        var payment = new Payment { PaymentID = 1, TransactionType = "credit" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

        var result = await _service.GetTransactionType(1, CancellationToken.None);

        result.Should().Be(payment);
    }

    [Fact]
    public async Task UpdateCredits_AddsCreditAndReturnsSuccess()
    {
        var userCredit = new UserCredit { UserId = 1, CreditBalance = 50 };
        _repositoryMock.Setup(r => r.GetBalanceByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userCredit);
        _repositoryMock.Setup(r => r.UpdateCredits(userCredit, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateCredits(1, 100, CancellationToken.None);

        result.Should().Be("Success");
    }

    [Fact]
    public async Task CreateBalanceAsync_CreatesBalance()
    {
        var balance = new UserCredit { UserId = 1, CreditBalance = 500 };
        _repositoryMock.Setup(r => r.AddBalanceAsync(balance, It.IsAny<CancellationToken>())).ReturnsAsync(456);

        var result = await _service.CreateBalanceAsync(balance, CancellationToken.None);

        result.Should().Be(456);
    }
}
