using System;
using System.Threading.Tasks;
using AdminSettings.Controllers;
using AdminSettings.Data;
using AdminSettings.Persistence.Entities;
using AdminSettings.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminSettings.Tests
{
    public class BackupControllerIntegrationTests
    {
        private AdminSettingsDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AdminSettingsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AdminSettingsDbContext(options);
        }

        [Fact]
        public async Task RunBackup_NoSettings_ReturnsServerError()
        {
            await using var context = CreateContext();
            var systemService = new SystemSettingsService(context);
            var service = new DatabaseBackupService(systemService);
            var controller = new BackupController(service);

            var result = await controller.RunBackup();

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
            Assert.Equal("❌ System settings not found. Backup cannot be performed.", obj.Value);
        }

        [Fact]
        public async Task RunBackup_ManualDisabled_ReturnsBadRequest()
        {
            await using var context = CreateContext();
            var settings = new SystemSetting
            {
                AuditLogEnabled = false,
                NotificationEnabled = false,
                DatabaseBackupSetting = new DatabaseBackupSetting
                {
                    ManualBackupEnabled = false,
                    AutomaticBackupEnabled = false
                }
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();

            var systemService = new SystemSettingsService(context);
            var service = new DatabaseBackupService(systemService);
            var controller = new BackupController(service);

            var result = await controller.RunBackup();

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("❌ Manual backup is disabled. Please enable it to perform a backup.", bad.Value);
        }

        [Fact]
        public async Task RunBackup_ManualEnabled_ReturnsOk()
        {
            await using var context = CreateContext();
            var settings = new SystemSetting
            {
                AuditLogEnabled = false,
                NotificationEnabled = false,
                DatabaseBackupSetting = new DatabaseBackupSetting
                {
                    ManualBackupEnabled = true,
                    AutomaticBackupEnabled = false
                }
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();

            var systemService = new SystemSettingsService(context);
            var service = new DatabaseBackupService(systemService);
            var controller = new BackupController(service);

            var result = await controller.RunBackup();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("✅ Backup completed.", ok.Value);
        }
    }
}
