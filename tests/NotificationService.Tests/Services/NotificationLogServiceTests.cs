using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using src.NotificationService.API.Persistence.Entities.DB.Models;
using src.NotificationService.API.Services;
using NotificationService.API.Persistence.Entities.DB;

namespace NotificationService.Tests.Services
{
    public class NotificationLogServiceTests
    {
        private DbContextOptions<NotificationDbContext> CreateInMemoryOptions(string dbName) =>
            new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

        private NotificationDbContext CreateContext(string dbName)
        {
            var options = CreateInMemoryOptions(dbName);
            return new NotificationDbContext(options);
        }

        /// <summary>
        /// Tests that LogNotification correctly stores a notification in the database.
        /// </summary>
        [Fact]
        public async Task LogNotification_AddsEntryToDatabase()
        {
            using var context = CreateContext(nameof(LogNotification_AddsEntryToDatabase));
            var service = new NotificationLogService(context);

            await service.LogNotification(1, NotificationType.EmailOther, "Subject", "Text");

            var result = await context.NotificationLogs.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal(NotificationType.EmailOther, result.NotificationType);
            Assert.Equal("Subject", result.Subject);
            Assert.False(result.HasBeeenRead);
        }

        /// <summary>
        /// Tests that GetNotificationsFor returns only unread notifications by default.
        /// </summary>
        [Fact]
        public async Task GetNotificationsFor_ReturnsOnlyUnreadByDefault()
        {
            using var context = CreateContext(nameof(GetNotificationsFor_ReturnsOnlyUnreadByDefault));

            context.NotificationLogs.AddRange(
                new NotificationLog
                {
                    UserId = 1,
                    HasBeeenRead = false,
                    SentDate = DateTime.UtcNow,
                    Subject = "Unread",
                    Text = "Text",
                    NotificationType = NotificationType.ReservationConfirmed
                },
                new NotificationLog
                {
                    UserId = 1,
                    HasBeeenRead = true,
                    SentDate = DateTime.UtcNow,
                    Subject = "Read",
                    Text = "Text",
                    NotificationType = NotificationType.ReservationConfirmed
                }
            );
            await context.SaveChangesAsync();

            var service = new NotificationLogService(context);
            var result = await service.GetNotificationsFor(1);

            Assert.Single(result);
            Assert.False(result[0].HasBeeenRead);
        }

    }
}
