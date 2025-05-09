using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AdminSettings.Services;
using Microsoft.EntityFrameworkCore;
using AdminSettings.Data;
using AdminSettings.Persistence.Entities;
using AdminSettings.Persistence.Enums;

namespace AdminSettings.Tests;

public class SystemSettingsServiceTests
{
    private Mock<AdminSettingsDbContext> CreateDbContextMock()
    {
        var dbName = $"TestDB_{Guid.NewGuid()}"; // Unikátny názov databázy pre každý test
        var options = new DbContextOptionsBuilder<AdminSettingsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var context = new AdminSettingsDbContext(options);
        return new Mock<AdminSettingsDbContext>(options) { CallBase = true };
    }

    [Fact]
    public async Task GetSystemSettingsAsync_ReturnsSettings_WhenExist()
    {
        var dbContextMock = CreateDbContextMock();
        var settings = new SystemSetting
        {
            AuditLogEnabled = true,
            NotificationEnabled = true,
            DatabaseBackupSetting = new DatabaseBackupSetting()
        };
        dbContextMock.Object.SystemSettings.Add(settings);
        await dbContextMock.Object.SaveChangesAsync();

        var service = new SystemSettingsService(dbContextMock.Object);

        var result = await service.GetSystemSettingsAsync();

        Assert.NotNull(result);
        Assert.True(result.AuditLogEnabled);
    }

    [Fact]
    public async Task GetSystemSettingsAsync_ReturnsNull_WhenNotExist()
    {
        var dbContextMock = CreateDbContextMock();
        var service = new SystemSettingsService(dbContextMock.Object);

        var result = await service.GetSystemSettingsAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateSystemSettingsAsync_UpdatesSuccessfully()
    {
        var dbContextMock = CreateDbContextMock();
        var settings = new SystemSetting
        {
            AuditLogEnabled = false,
            NotificationEnabled = false,
            DatabaseBackupSetting = new DatabaseBackupSetting()
        };
        dbContextMock.Object.SystemSettings.Add(settings);
        await dbContextMock.Object.SaveChangesAsync();

        var service = new SystemSettingsService(dbContextMock.Object);
        var updatedSettings = new SystemSetting
        {
            AuditLogEnabled = true,
            NotificationEnabled = true,
            DatabaseBackupSetting = new DatabaseBackupSetting()
        };

        var result = await service.UpdateSystemSettingsAsync(updatedSettings);

        Assert.True(result);
    }

    [Fact]
    public async Task GetAuditLogEnabledAsync_ReturnsTrue_WhenEnabled()
    {
        var dbContextMock = CreateDbContextMock();
        dbContextMock.Object.SystemSettings.Add(new SystemSetting
        {
            AuditLogEnabled = true,
            NotificationEnabled = true,
            DatabaseBackupSetting = new DatabaseBackupSetting()
        });
        await dbContextMock.Object.SaveChangesAsync();

        var service = new SystemSettingsService(dbContextMock.Object);

        var result = await service.GetAuditLogEnabledAsync();

        Assert.True(result);
    }

    [Fact]
    public async Task GetNotificationEnabledAsync_ReturnsFalse_WhenDisabled()
    {
        var dbContextMock = CreateDbContextMock();
        dbContextMock.Object.SystemSettings.Add(new SystemSetting
        {
            AuditLogEnabled = false,
            NotificationEnabled = false,
            DatabaseBackupSetting = new DatabaseBackupSetting()
        });
        await dbContextMock.Object.SaveChangesAsync();

        var service = new SystemSettingsService(dbContextMock.Object);

        var result = await service.GetNotificationEnabledAsync();

        Assert.False(result);
    }
}
