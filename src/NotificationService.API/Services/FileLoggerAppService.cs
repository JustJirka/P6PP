using System.Text;

namespace NotificationService.API.Logging;

public static class FileLogger
{
    private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "errors.log");

    public static void LogError(string message, Exception? ex = null)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath)!);

            var log = new StringBuilder();
            log.AppendLine("----- ERROR -----");
            log.AppendLine($"Timestamp: {DateTime.Now}");
            log.AppendLine($"Message: {message}");

            if (ex != null)
            {
                log.AppendLine($"Exception: {ex.Message}");
                log.AppendLine($"StackTrace: {ex.StackTrace}");
            }

            log.AppendLine();

            File.AppendAllText(LogFilePath, log.ToString());
        }
        catch
        {
            
        }

    }
    public static async Task LogInfo(string message)
    {
        var log = new StringBuilder();
        log.AppendLine($"[{DateTime.Now}] INFO: {message}");
        log.AppendLine(new string('-', 80));

        Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath)!);
        await File.AppendAllTextAsync(LogFilePath, log.ToString());
    }

    // Logování výjimek
    public static async Task LogException(Exception ex, string context)
    {
        var log = new StringBuilder();
        log.AppendLine($"[{DateTime.Now}] ERROR in context: {context}");
        log.AppendLine($"Message: {ex.Message}");
        log.AppendLine($"StackTrace: {ex.StackTrace}");
        log.AppendLine(new string('-', 80));

        Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath)!);
        await File.AppendAllTextAsync(LogFilePath, log.ToString());
    }
}
