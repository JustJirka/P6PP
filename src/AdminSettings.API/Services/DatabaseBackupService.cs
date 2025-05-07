using AdminSettings.Persistence.Entities;
using AdminSettings.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class DatabaseBackupService
{
    private readonly SystemSettingsService _systemSettingsService;
    private readonly IConfiguration _configuration;


    public DatabaseBackupService(SystemSettingsService systemSettingsService, IConfiguration configuration)
    {
        _systemSettingsService = systemSettingsService;
        _configuration = configuration;
    }

    public async Task<bool> BackupDatabaseAsync(string dbName, string user, string password)
    {
        string backupFilePath = $"/backups/{dbName}_backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";

        var startInfo = new ProcessStartInfo
        {
            FileName = "mysqldump",
            Arguments = $"-h mysql -u{user} -p{password} {dbName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = new Process { StartInfo = startInfo };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                // 💾 Zapsání výstupu do souboru
                await File.WriteAllTextAsync(backupFilePath, output);
                Console.WriteLine($"✅ Backup {dbName} saved to {backupFilePath}");
                
                return true;
            }
            else
            {
                Console.WriteLine($"❌ Error while backing up {dbName}: {error}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception for backup {dbName}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> BackupAllAsync()
    {
        var systemSettings = await _systemSettingsService.GetSystemSettingsAsync();

        if (systemSettings?.DatabaseBackupSetting == null)
        {
            Console.WriteLine("❌ Backup settings not found.");
            return false;
        }

        if (!systemSettings.DatabaseBackupSetting.ManualBackupEnabled)
        {
            Console.WriteLine("❌ Backup not allowed.");
            return false;
        }

        var databases = _configuration.GetSection("DatabaseBackup:Databases").Get<List<DatabaseCredential>>();

        if (databases == null || databases.Count == 0)
        {
            Console.WriteLine("❌ No databases configured for backup.");
            return false;
        }

        foreach (var db in databases)
        {
            await BackupDatabaseAsync(db.DbName, db.User, db.Password);
        }

        return true;
    }

    public async Task<SystemSetting?> GetSystemSettingsAsync()
    {
        return await _systemSettingsService.GetSystemSettingsAsync();
    }

    public class DatabaseCredential
    {
        public required string DbName { get; set; }
        public required string User { get; set; }
        public required string Password { get; set; }
    }
}