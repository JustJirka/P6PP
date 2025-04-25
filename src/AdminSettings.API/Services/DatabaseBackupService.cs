using System.Diagnostics;

public class DatabaseBackupService
{
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
                Console.WriteLine($"✅ Záloha {dbName} uložena do {backupFilePath}");
                return true;
            }
            else
            {
                Console.WriteLine($"❌ Chyba při záloze {dbName}: {error}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Výjimka při záloze {dbName}: {ex.Message}");
            return false;
        }
    }

    public async Task BackupAllAsync()
    {
        var databases = new List<(string DbName, string User, string Password)>
        {
            ("admin_db", "root", "password123"),
            ("auth_db", "root", "password123"),
            ("userdb", "root", "password123")
        };

        foreach (var db in databases)
        {
            await BackupDatabaseAsync(db.DbName, db.User, db.Password);
        }
    }
}