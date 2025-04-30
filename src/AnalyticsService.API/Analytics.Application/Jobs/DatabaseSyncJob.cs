using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analytics.Application.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Analytics.Application.Jobs
{
    public class DatabaseSyncJob : IJob
    {
        private readonly DatabaseSyncService _syncService;
        private readonly ILogger<DatabaseSyncJob> _logger;

        public DatabaseSyncJob(DatabaseSyncService syncService, ILogger<DatabaseSyncJob> logger)
        {
            _syncService = syncService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("DatabaseSyncJob triggered at {Time}", DateTime.UtcNow);
            await _syncService.SyncDatabase();
        }
    }
}
