using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Configuration;
using Moq;
using NotificationService.API.Abstraction;
using NotificationService.API.Persistence;
using NotificationService.API.Persistence.Entities;
using NotificationService.API.Persistence.Entities.DB.Models;
using NotificationService.API.Services;
using ReservationSystem.Shared.Abstraction;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Benchmarks
{
    [MemoryDiagnoser]
    public class BookingAppServiceBenchmarks
    {
        private BookingAppService _service;
        private readonly Booking _booking = new() { BookingId = 1, Start = DateTime.UtcNow };

        [GlobalSetup]
        public void Setup()
        {
            var httpClientMock = new Mock<INetworkHttpClient>();
            var dbContextMock = new Mock<INotificationDbContext>();
            var userAppServiceMock = new Mock<IUserAppService>();
            var templateAppServiceMock = new Mock<ITemplateAppService>();
            var mailAppServiceMock = new Mock<IMailAppService>();
            var configurationMock = new Mock<IConfiguration>();

            // Setup expected responses
            var bookingResp = new BookingResponse(1, 2, "Confirmed", 3);
            var serviceResp = new ServiceResponse(
                2,
                DateTime.UtcNow.AddHours(23),
                DateTime.UtcNow.AddHours(24),
                150,
                "Reminder Test",
                2,
                5,
                "Main Hall",
                false
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

            // Mock all expected service interactions
            httpClientMock.Setup(x => x.GetAsync<BookingResponse>(It.IsAny<string>(), default))
                .ReturnsAsync(new ApiResult<BookingResponse>(bookingResp));

            httpClientMock.Setup(x => x.GetAsync<ServiceResponse>(It.IsAny<string>(), default))
                .ReturnsAsync(new ApiResult<ServiceResponse>(serviceResp));

            userAppServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
            templateAppServiceMock.Setup(x => x.GetTemplateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(template);
            mailAppServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailArgs>(), null)).Returns(Task.CompletedTask);

            _service = new BookingAppService(
                httpClientMock.Object,
                dbContextMock.Object,
                userAppServiceMock.Object,
                templateAppServiceMock.Object,
                mailAppServiceMock.Object,
                configurationMock.Object
            );
        }

        [Benchmark]
        public async Task Benchmark_GetBookingByIdAsync()
        {
            await _service.GetBookingByIdAsync(1);
        }

        [Benchmark]
        public async Task Benchmark_GetServiceByIdAsync()
        {
            await _service.GetServiceByIdAsync(1);
        }

        [Benchmark]
        public async Task Benchmark_GetServiceByBookingIdAsync()
        {
            await _service.GetServiceByBookingIdAsync(1);
        }

        [Benchmark]
        public async Task Benchmark_SendReminderEmail()
        {
            await _service.SendReminderEmail(_booking);
        }
    }
    // Optional: entry point to run directly
    public class Program
    {
        public static void Main(string[] args)
        {
            // dotnet run -c Release
            BenchmarkRunner.Run<BookingAppServiceBenchmarks>();
        }
    }
}
