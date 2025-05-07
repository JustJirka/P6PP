
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AdminSettings.Services;
using AdminSettings.Persistence.Entities;
using AdminSettings.Persistence.Enums;

namespace AdminSettings.Services
{
    public class BackupSchedulerService : BackgroundService
    {
        private readonly ILogger<BackupSchedulerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        private readonly int _loopDelayMinutes;
        private readonly int _postBackupDelayMinutes;
        private readonly int _errorDelayMinutes;
        private readonly int _timeCheckWindowMinutes;

        public BackupSchedulerService(ILogger<BackupSchedulerService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;

            _loopDelayMinutes = _configuration.GetValue<int>("BackupScheduler:LoopDelayMinutes");
            _postBackupDelayMinutes = _configuration.GetValue<int>("BackupScheduler:PostBackupDelayMinutes");
            _errorDelayMinutes = _configuration.GetValue<int>("BackupScheduler:ErrorDelayMinutes");
            _timeCheckWindowMinutes = _configuration.GetValue<int>("BackupScheduler:TimeCheckWindowMinutes");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var systemSettingsService = scope.ServiceProvider.GetRequiredService<SystemSettingsService>();
                        var backupService = scope.ServiceProvider.GetRequiredService<DatabaseBackupService>();

                        var systemSettings = await systemSettingsService.GetSystemSettingsAsync();

                        if (systemSettings?.DatabaseBackupSetting != null && systemSettings.DatabaseBackupSetting.AutomaticBackupEnabled)
                        {
                            var backupSetting = systemSettings.DatabaseBackupSetting;

                            if (ShouldRunBackup(backupSetting))
                            {
                                _logger.LogInformation("Running a scheduled database backup.");
                                await backupService.BackupAllAsync();
                                await Task.Delay(TimeSpan.FromMinutes(_postBackupDelayMinutes), stoppingToken);
                            }
                        }
                    }

                    await Task.Delay(TimeSpan.FromMinutes(_loopDelayMinutes), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while scheduling backups.");
                    await Task.Delay(TimeSpan.FromMinutes(_errorDelayMinutes), stoppingToken);
                }
            }
        }

        private bool ShouldRunBackup(DatabaseBackupSetting settings)
        {
            DateTime now = DateTime.Now;
            TimeOnly currentTime = TimeOnly.FromDateTime(now);

            bool isTimeToBackup = Math.Abs((currentTime.ToTimeSpan() - settings.BackupTime.ToTimeSpan()).TotalMinutes) < _timeCheckWindowMinutes;

            if (!isTimeToBackup)
                return false;

            return settings.BackupFrequency switch
            {
                BackupFrequency.Daily => true,
                BackupFrequency.Weekly => now.DayOfWeek == DayOfWeek.Sunday,
                BackupFrequency.Monthly => now.Day == 1,
                _ => false
            };
        }
    }
}
