using Xunit;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AdminSettings.Services;
using AdminSettings.Data;
using AdminSettings.Persistence.Entities;
using AdminSettings.Persistence.Enums;

namespace AdminSettings.Tests;

public class DatabaseBackupServiceTests
{
    private AdminSettingsDbContext CreateDbContext()
    {
        var dbName = $"TestDB_{Guid.NewGuid()}"; // Unikátny názov databázy pre každý test
        var options = new DbContextOptionsBuilder<AdminSettingsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AdminSettingsDbContext(options);
    }


    [Fact]
    public async Task BackupDatabaseAsync_ReturnsFalse_WhenBackupFails()
    {
        var dbContext = CreateDbContext();
        dbContext.SystemSettings.Add(new SystemSetting
        {
            AuditLogEnabled = true,
            NotificationEnabled = true,
            DatabaseBackupSetting = new DatabaseBackupSetting { ManualBackupEnabled = true }
        });
        await dbContext.SaveChangesAsync();

        var systemSettingsService = new SystemSettingsService(dbContext);
        var service = new DatabaseBackupService(systemSettingsService);

        bool result = await service.BackupDatabaseAsync("invalid_db", "root", "wrongpassword");

        Assert.False(result);
    }

    [Fact]
    public async Task BackupDatabaseAsync_ReturnsFalse_OnException()
    {
        var dbContext = CreateDbContext();
        dbContext.SystemSettings.Add(new SystemSetting
        {
            AuditLogEnabled = true,
            NotificationEnabled = true,
            DatabaseBackupSetting = new DatabaseBackupSetting { ManualBackupEnabled = true }
        });
        await dbContext.SaveChangesAsync();

        var systemSettingsService = new SystemSettingsService(dbContext);
        var service = new DatabaseBackupService(systemSettingsService);

        bool result = await service.BackupDatabaseAsync("", "", "");

        Assert.False(result);
    }

    [Fact]
    public async Task BackupAllAsync_ReturnsFalse_WhenSettingsNotFound()
    {
        var dbContext = CreateDbContext();
        var systemSettingsService = new SystemSettingsService(dbContext);
        var service = new DatabaseBackupService(systemSettingsService);

        bool result = await service.BackupAllAsync();

        Assert.False(result);
    }

    [Fact]
    public async Task BackupAllAsync_ReturnsFalse_WhenBackupDisabled()
    {
        var dbContext = CreateDbContext();
        dbContext.SystemSettings.Add(new SystemSetting
        {
            AuditLogEnabled = true,
            NotificationEnabled = true,
            DatabaseBackupSetting = new DatabaseBackupSetting { ManualBackupEnabled = false }
        });
        await dbContext.SaveChangesAsync();

        var systemSettingsService = new SystemSettingsService(dbContext);
        var service = new DatabaseBackupService(systemSettingsService);

        bool result = await service.BackupAllAsync();

        Assert.False(result);
    }

    [Fact]
    public async Task BackupAllAsync_ReturnsTrue_WhenBackupEnabled()
    {
        var dbContext = CreateDbContext();
        dbContext.SystemSettings.Add(new SystemSetting
        {
            AuditLogEnabled = true,
            NotificationEnabled = true,
            DatabaseBackupSetting = new DatabaseBackupSetting { ManualBackupEnabled = true }
        });
        await dbContext.SaveChangesAsync();

        var systemSettingsService = new SystemSettingsService(dbContext);
        var service = new DatabaseBackupService(systemSettingsService);

        bool result = await service.BackupAllAsync();

        Assert.True(result);
    }
}
