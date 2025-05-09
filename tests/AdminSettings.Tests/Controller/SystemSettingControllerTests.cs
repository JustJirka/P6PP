using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AdminSettings.Controllers;
using AdminSettings.Data;
using AdminSettings.Persistence.Entities;
using AdminSettings.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminSettings.Tests
{
    public class SystemSettingsControllerTests
    {
        private AdminSettingsDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AdminSettingsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AdminSettingsDbContext(options);
        }

        [Fact]
        public async Task GetSystemSettings_KeďExistujú_NávratOkSoSettings()
        {
            await using var context = CreateContext();
            var settings = new SystemSetting
            {
                AuditLogEnabled = true,
                NotificationEnabled = false,
                DatabaseBackupSetting = new DatabaseBackupSetting()
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();

            var service = new SystemSettingsService(context);
            var controller = new SystemSettingsController(service);

            var result = await controller.GetSystemSettings();

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<SystemSetting>(ok.Value);
            Assert.True(returned.AuditLogEnabled);
        }

        [Fact]
        public async Task GetSystemSettings_KeďNeexistujú_NávratNotFound()
        {
            await using var context = CreateContext();
            var controller = new SystemSettingsController(new SystemSettingsService(context));

            var result = await controller.GetSystemSettings();

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("System settings not found.", nf.Value);
        }

        [Fact]
        public async Task UpdateSystemSettings_InvalidModel_ReturnsBadRequest()
        {
            await using var context = CreateContext();
            var controller = new SystemSettingsController(new SystemSettingsService(context));
            controller.ModelState.AddModelError("x", "Chybný model");

            var result = await controller.UpdateSystemSettings(null);

            var br = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid data.", br.Value);
        }

        [Fact]
        public async Task UpdateSystemSettings_KeďUspešné_NávratNoContent()
        {
            await using var context = CreateContext();
            var settings = new SystemSetting
            {
                AuditLogEnabled = false,
                NotificationEnabled = true,
                DatabaseBackupSetting = new DatabaseBackupSetting()
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();

            // Change and update
            settings.AuditLogEnabled = true;
            var controller = new SystemSettingsController(new SystemSettingsService(context));

            var result = await controller.UpdateSystemSettings(settings);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateSystemSettings_KeďNenašlo_NávratNotFound()
        {
            await using var context = CreateContext();
            var fake = new SystemSetting
            {
                Id = 999,
                AuditLogEnabled = true,
                NotificationEnabled = true,
                DatabaseBackupSetting = new DatabaseBackupSetting()
            };
            var controller = new SystemSettingsController(new SystemSettingsService(context));

            var result = await controller.UpdateSystemSettings(fake);

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("System settings not found.", nf.Value);
        }

        [Fact]
        public async Task GetDatabaseBackupSetting_NavraciaOkSoBackupSetting()
        {
            await using var context = CreateContext();
            var initial = new SystemSetting
            {
                AuditLogEnabled = false,
                NotificationEnabled = false,
                DatabaseBackupSetting = new DatabaseBackupSetting { ManualBackupEnabled = true }
            };
            context.SystemSettings.Add(initial);
            await context.SaveChangesAsync();

            var controller = new SystemSettingsController(new SystemSettingsService(context));
            var result = await controller.GetDatabaseBackupSetting();

            var ok = Assert.IsType<OkObjectResult>(result);
            // Service returns List<DatabaseBackupSetting>
            var list = Assert.IsType<List<DatabaseBackupSetting>>(ok.Value);
            Assert.Single(list);
            Assert.True(list[0].ManualBackupEnabled);
        }

        [Fact]
        public async Task UpdateDatabaseBackupSetting_InvalidModel_ReturnsBadRequest()
        {
            await using var context = CreateContext();
            var controller = new SystemSettingsController(new SystemSettingsService(context));
            controller.ModelState.AddModelError("x", "Chybný model");

            var result = await controller.UpdateDatabaseBackupSetting(null);

            var br = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid backup setting data.", br.Value);
        }

        [Fact]
        public async Task UpdateDatabaseBackupSetting_KeďUspešné_NávratNoContent()
        {
            await using var context = CreateContext();
            var settings = new SystemSetting
            {
                AuditLogEnabled = false,
                NotificationEnabled = false,
                DatabaseBackupSetting = new DatabaseBackupSetting { ManualBackupEnabled = false }
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();

            // Modify tracked instance
            settings.DatabaseBackupSetting.ManualBackupEnabled = true;
            var controller = new SystemSettingsController(new SystemSettingsService(context));

            var result = await controller.UpdateDatabaseBackupSetting(settings.DatabaseBackupSetting);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateDatabaseBackupSetting_KeďNenašlo_NávratNotFound()
        {
            await using var context = CreateContext();
            var fake = new DatabaseBackupSetting();
            var controller = new SystemSettingsController(new SystemSettingsService(context));

            var result = await controller.UpdateDatabaseBackupSetting(fake);

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Backup setting not found.", nf.Value);
        }

        [Fact]
        public async Task GetAuditLogEnabled_NavraciaOkSoBool()
        {
            await using var context = CreateContext();
            var settings = new SystemSetting
            {
                AuditLogEnabled = true,
                NotificationEnabled = false,
                DatabaseBackupSetting = new DatabaseBackupSetting()
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();

            var result = await new SystemSettingsController(new SystemSettingsService(context)).GetAuditLogEnabled();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)ok.Value);
        }

        [Fact]
        public async Task SetAuditLogEnabled_KeďNeexistujú_NávratNotFound()
        {
            await using var context = CreateContext();
            var result = await new SystemSettingsController(new SystemSettingsService(context)).SetAuditLogEnabled(true);

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            // returns anonymous, so check ToString()
            Assert.Contains("System settings not found", nf.Value.ToString());
        }

        [Fact]
        public async Task SetAuditLogEnabled_KeďUspešné_NávratOkMessage()
        {
            await using var context = CreateContext();
            var settings = new SystemSetting
            {
                AuditLogEnabled = false,
                NotificationEnabled = false,
                DatabaseBackupSetting = new DatabaseBackupSetting()
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();

            var result = await new SystemSettingsController(new SystemSettingsService(context)).SetAuditLogEnabled(true);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Audit log has been enabled", ok.Value.ToString());
        }

        [Fact]
        public async Task GetNotificationEnabled_NavraciaOkSoBool()
        {
            await using var context = CreateContext();
            var settings = new SystemSetting
            {
                AuditLogEnabled = false,
                NotificationEnabled = true,
                DatabaseBackupSetting = new DatabaseBackupSetting()
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();

            var result = await new SystemSettingsController(new SystemSettingsService(context)).GetNotificationEnabled();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)ok.Value);
        }

        [Fact]
        public async Task SetManualBackupEnabled_KeďNeexistujú_NávratNotFound()
        {
            await using var context = CreateContext();
            var result = await new SystemSettingsController(new SystemSettingsService(context)).SetManualBackupEnabled(true);

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("System settings not found", nf.Value.ToString());
        }

        [Fact]
        public async Task SetManualBackupEnabled_KeďUspešné_NávratOkMessage()
        {
            await using var context = CreateContext();
            var settings = new SystemSetting
            {
                AuditLogEnabled = false,
                NotificationEnabled = false,
                DatabaseBackupSetting = new DatabaseBackupSetting { ManualBackupEnabled = false }
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();

            var result = await new SystemSettingsController(new SystemSettingsService(context)).SetManualBackupEnabled(true);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Manual backup has been enabled", ok.Value.ToString());
        }

        [Fact]
        public async Task SetAutomaticBackupEnabled_KeďNeexistujú_NávratNotFound()
        {
            await using var context = CreateContext();
            var result = await new SystemSettingsController(new SystemSettingsService(context)).SetAutomaticBackupEnabled(true);

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("System settings not found", nf.Value.ToString());
        }

        [Fact]
        public async Task SetAutomaticBackupEnabled_KeďUspešné_NávratOkMessage()
        {
            await using var context = CreateContext();
            var settings = new SystemSetting
            {
                AuditLogEnabled = false,
                NotificationEnabled = false,
                DatabaseBackupSetting = new DatabaseBackupSetting { AutomaticBackupEnabled = false }
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();

            var result = await new SystemSettingsController(new SystemSettingsService(context)).SetAutomaticBackupEnabled(true);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Automatic backup has been enabled", ok.Value.ToString());
        }
    }
}