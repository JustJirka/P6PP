using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using NotificationService.API.Services;
using NotificationService.API.Persistence.Entities;
using ReservationSystem.Shared.Abstraction;
using ReservationSystem.Shared.Results;

namespace NotificationService.Tests.Services
{
    public class UserAppServiceTests
    {
        /// <summary>
        /// Tests that GetUserByIdAsync returns the user object when the API client returns valid data.
        /// </summary>
        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenApiReturnsData()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "jdoe",
                FirstName = "John",
                LastName = "Doe",
                Email = "jdoe@example.com",
                CreatedOn = DateTime.UtcNow
            };

            var result = new ApiResult<UserAppService.GetUserRespond>(
                new UserAppService.GetUserRespond(user)
            );

            var httpClientMock = new Mock<INetworkHttpClient>();
            httpClientMock
                .Setup(c => c.GetAsync<UserAppService.GetUserRespond>(It.IsAny<string>(), default))
                .ReturnsAsync(result);

            var service = new UserAppService(httpClientMock.Object);

            // Act
            var retrievedUser = await service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Id, retrievedUser!.Id);
            Assert.Equal(user.Username, retrievedUser.Username);
            Assert.Equal(user.FirstName, retrievedUser.FirstName);
            Assert.Equal(user.Email, retrievedUser.Email);
        }

        /// <summary>
        /// Tests that GetUserByIdAsync returns null when the API client returns no data.
        /// </summary>
        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenApiDataIsNull()
        {
            // Arrange
            var result = new ApiResult<UserAppService.GetUserRespond>(null);

            var httpClientMock = new Mock<INetworkHttpClient>();
            httpClientMock
                .Setup(c => c.GetAsync<UserAppService.GetUserRespond>(It.IsAny<string>(), default))
                .ReturnsAsync(result);

            var service = new UserAppService(httpClientMock.Object);

            // Act
            var retrievedUser = await service.GetUserByIdAsync(999);

            // Assert
            Assert.Null(retrievedUser);
        }

        /// <summary>
        /// Tests that GetUserByIdAsync returns null when the API client throws an exception.
        /// </summary>
        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenHttpClientThrows()
        {
            // Arrange
            var httpClientMock = new Mock<INetworkHttpClient>();
            httpClientMock
                .Setup(c => c.GetAsync<UserAppService.GetUserRespond>(It.IsAny<string>(), default))
                .ThrowsAsync(new Exception("Simulated failure"));

            var service = new UserAppService(httpClientMock.Object);

            // Act
            var retrievedUser = await service.GetUserByIdAsync(123);

            // Assert
            Assert.Null(retrievedUser);
        }
    }
}
