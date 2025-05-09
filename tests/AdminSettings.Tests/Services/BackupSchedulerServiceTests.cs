using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using AdminSettings.Services;
using AdminSettings.Persistence.Entities;
using AdminSettings.Persistence.Enums;

namespace AdminSettings.Tests;

public class BackupSchedulerServiceTests
{
    public class FakeSystemSettingsService
    {
        private readonly SystemSetting _setting;

        public FakeSystemSettingsService(SystemSetting setting)
        {
            _setting = setting;
        }

        public Task<SystemSetting> GetSystemSettingsAsync()
        {
            return Task.FromResult(_setting);
        }
    }

    public class FakeDatabaseBackupService
    {
        public bool WasBackupCalled { get; private set; }

        public Task<bool> BackupAllAsync()
        {
            WasBackupCalled = true;
            Console.WriteLine("BackupAllAsync called");
            return Task.FromResult(true);
        }
    }


    [Fact]
    public void ShouldRunBackup_ReturnsTrue_WhenDailyAndTimeMatches()
    {
        var service = new BackupSchedulerService(null, null);
        var settings = new DatabaseBackupSetting
        {
            BackupFrequency = BackupFrequency.Daily,
            BackupTime = TimeOnly.FromDateTime(DateTime.Now)
        };

        var methodInfo = typeof(BackupSchedulerService).GetMethod("ShouldRunBackup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bool result = (bool)methodInfo.Invoke(service, new object[] { settings });

        Assert.True(result);
    }

    [Fact]
    public void ShouldRunBackup_ReturnsFalse_WhenTimeDoesNotMatch()
    {
        var service = new BackupSchedulerService(null, null);
        var settings = new DatabaseBackupSetting
        {
            BackupFrequency = BackupFrequency.Daily,
            BackupTime = TimeOnly.FromDateTime(DateTime.Now.AddMinutes(10))
        };

        var methodInfo = typeof(BackupSchedulerService).GetMethod("ShouldRunBackup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bool result = (bool)methodInfo.Invoke(service, new object[] { settings });

        Assert.False(result);
    }

    [Fact]
    public async Task ExecuteAsync_LogsError_OnException()
    {
        var loggerMock = new Mock<ILogger<BackupSchedulerService>>();
        var serviceProviderMock = new Mock<IServiceProvider>();

        var fakeSystemSettingsService = new FakeSystemSettingsService(null); // Simulácia chyby

        serviceProviderMock.Setup(sp => sp.GetService(typeof(SystemSettingsService))).Returns(fakeSystemSettingsService);

        var service = new BackupSchedulerService(loggerMock.Object, serviceProviderMock.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100); // Rýchle ukončenie testu

        await service.StartAsync(cts.Token);

        loggerMock.Verify(l => l.Log(
            It.Is<LogLevel>(ll => ll == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while scheduling backups.")),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }
}
