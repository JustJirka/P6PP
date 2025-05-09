using Xunit;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq.Protected;
using AdminSettings.Services;
using AdminSettings.Persistence.Interface;

namespace AdminSettings.Tests;

public class UserServiceClientTests
{
    [Fact]
    public async Task UserExistsAsync_ReturnsTrue_WhenUserExists()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var service = new UserServiceClient(httpClient);

        // Act
        var result = await service.UserExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UserExistsAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var service = new UserServiceClient(httpClient);

        // Act
        var result = await service.UserExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UserExistsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("/api/user/1")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var service = new UserServiceClient(httpClient);

        // Act
        await service.UserExistsAsync(1);

        // Assert
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString() == "http://localhost/api/user/1"),
            ItExpr.IsAny<CancellationToken>());
    }
}
