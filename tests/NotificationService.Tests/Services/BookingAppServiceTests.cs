using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using NotificationService.API.Services;
using NotificationService.API.Abstraction;
using NotificationService.API.Persistence.Entities;
using NotificationService.API.Persistence.Entities.DB.Models;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared.Results;
using ReservationSystem.Shared.Abstraction;
using NotificationService.API.Persistence;
using Microsoft.Extensions.Configuration;


namespace NotificationService.Tests.Services
{
    public class BookingAppServiceTests
    {
        private readonly Mock<INetworkHttpClient> _httpClientMock = new();
        private readonly Mock<INotificationDbContext> _dbContextMock = new();
        private readonly Mock<IUserAppService> _userAppServiceMock = new();
        private readonly Mock<ITemplateAppService> _templateAppServiceMock = new();
        private readonly Mock<IMailAppService> _mailAppServiceMock = new();
        private readonly Mock<IConfiguration> _configurationMock = new();

        private BookingAppService CreateService() =>
            new BookingAppService(
                _httpClientMock.Object,
                _dbContextMock.Object,
                _userAppServiceMock.Object,
                _templateAppServiceMock.Object,
                _mailAppServiceMock.Object,
                _configurationMock.Object
            );

        /// <summary>
        /// Should return booking response when booking is found through API call.
        /// </summary>
        [Fact]
        public async Task GetBookingByIdAsync_ReturnsBooking_WhenHttpClientSucceeds()
        {
            var expected = new BookingResponse(id: 1, serviceId: 2, status: "Confirmed", userId: 3);
            _httpClientMock.Setup(x => x.GetAsync<BookingResponse>(It.IsAny<string>(), default))
                .ReturnsAsync(new ApiResult<BookingResponse>(expected));

            var service = CreateService();
            var result = await service.GetBookingByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(expected.id, result!.id);
        }

        /// <summary>
        /// Should return service response when called by ID.
        /// </summary>
        [Fact]
        public async Task GetServiceByIdAsync_ReturnsService_WhenHttpClientSucceeds()
        {
            var expected = new ServiceResponse(
                id: 1,
                start: DateTime.UtcNow,
                end: DateTime.UtcNow.AddHours(1),
                price: 100,
                serviceName: "Test Service",
                currentCapacity: 1,
                totalCapacity: 10,
                roomName: "Room A",
                isCancelled: false
            );
            _httpClientMock.Setup(x => x.GetAsync<ServiceResponse>(It.IsAny<string>(), default))
                .ReturnsAsync(new ApiResult<ServiceResponse>(expected));

            var service = CreateService();
            var result = await service.GetServiceByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(expected.id, result!.id);
        }

        /// <summary>
        /// Should fetch a service from booking ID through two API calls.
        /// </summary>
        [Fact]
        public async Task GetServiceByBookingIdAsync_ReturnsService_WhenBookingExists()
        {
            var booking = new BookingResponse(id: 1, serviceId: 5, status: "Confirmed", userId: 3);
            var serviceResp = new ServiceResponse(
                id: 5,
                start: DateTime.UtcNow,
                end: DateTime.UtcNow.AddHours(1),
                price: 200,
                serviceName: "Service 5",
                currentCapacity: 5,
                totalCapacity: 20,
                roomName: "Room B",
                isCancelled: false
            );

            _httpClientMock.Setup(x => x.GetAsync<BookingResponse>(It.IsAny<string>(), default))
                .ReturnsAsync(new ApiResult<BookingResponse>(booking));
            _httpClientMock.Setup(x => x.GetAsync<ServiceResponse>(It.IsAny<string>(), default))
                .ReturnsAsync(new ApiResult<ServiceResponse>(serviceResp));

            var service = CreateService();
            var result = await service.GetServiceByBookingIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(5, result!.id);
        }

        /// <summary>
        /// Sends reminder email if all services (user, template, email) succeed.
        /// </summary>
        [Fact]
        public async Task SendReminderEmail_ReturnsTrue_WhenEmailSentSuccessfully()
        {
            var bookingResp = new BookingResponse(id: 1, serviceId: 2, status: "Confirmed", userId: 3);
            var serviceResp = new ServiceResponse(
                id: 2,
                start: DateTime.UtcNow.AddHours(23),
                end: DateTime.UtcNow.AddHours(24),
                price: 150,
                serviceName: "Reminder Test",
                currentCapacity: 2,
                totalCapacity: 5,
                roomName: "Main Hall",
                isCancelled: false
            );
            var user = new User
            {
                Id = 3,
                FirstName = "Test",
                LastName = "User",
                Email = "user@example.com"
            };
            var template = new Template
            {
                Name = "BookingReminder",
                Subject = "Reminder",
                Text = "Dear {name}, your event {eventname} starts at {datetime}"
            };

            _httpClientMock.Setup(x => x.GetAsync<BookingResponse>(It.IsAny<string>(), default))
                .ReturnsAsync(new ApiResult<BookingResponse>(bookingResp));

            _httpClientMock.Setup(x => x.GetAsync<ServiceResponse>(It.IsAny<string>(), default))
                .ReturnsAsync(new ApiResult<ServiceResponse>(serviceResp));

            _userAppServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);

            _templateAppServiceMock.Setup(x => x.GetTemplateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(template);

            _mailAppServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailArgs>(), null))
                .Returns(Task.CompletedTask);

            var service = CreateService();
            var result = await service.SendReminderEmail(new Booking { BookingId = 1, Start = DateTime.UtcNow });

            Assert.True(result);
        }
    }
}
