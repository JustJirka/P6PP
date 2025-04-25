using Dapper;
using MySqlConnector;

namespace AdminSettings.Persistence;

public class DatabaseInitializer
{
    private readonly string _connectionString;
    private readonly string _adminConnectionString;
    private readonly string _databaseName;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                              ?? throw new Exception("Connection string not found.");

        _logger = logger;

        var builder = new MySqlConnectionStringBuilder(_connectionString);
        _databaseName = builder.Database;
        builder.Database = ""; // Přepnutí na root úroveň
        _adminConnectionString = builder.ToString();
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("Checking for existence of database '{Database}'...", _databaseName);

            await using var adminConn = new MySqlConnection(_adminConnectionString);
            await adminConn.OpenAsync();
            _logger.LogInformation("Admin database connection opened successfully.");
            var dbExists = await adminConn.ExecuteScalarAsync<string>(
                "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @DatabaseName",
                new { DatabaseName = _databaseName });

            if (dbExists == null)
            {
                _logger.LogInformation("Creating database '{Database}'...", _databaseName);
                await adminConn.ExecuteAsync($"CREATE DATABASE `{_databaseName}`;");
                _logger.LogInformation("Database '{Database}' created.", _databaseName);
            }

            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            await CreateAuditLogsTableAsync(conn);
            await CreateTimezoneTableAsync(conn);
            await CreateCurrencyTableAsync(conn);
            await CreateSystemSettingsTableAsync(conn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed.");
            throw;
        }
    }

    private async Task CreateAuditLogsTableAsync(MySqlConnection conn)
    {
        const string tableSql = @"
        CREATE TABLE IF NOT EXISTS AuditLogs (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            UserId VARCHAR(255) NOT NULL,
            TimeStamp DATETIME NOT NULL,
            Action VARCHAR(255) NOT NULL
        );";

        await conn.ExecuteAsync(tableSql);
        _logger.LogInformation("Table 'AuditLogs' ensured.");
    }

    private async Task CreateTimezoneTableAsync(MySqlConnection conn)
    {
        const string tableSql = @"
        CREATE TABLE IF NOT EXISTS Timezones (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Name VARCHAR(255) NOT NULL,
            UtcOffset VARCHAR(255) NOT NULL
        );";

        await conn.ExecuteAsync(tableSql);
        _logger.LogInformation("Table 'Timezone' ensured.");
    }

    private async Task CreateCurrencyTableAsync(MySqlConnection conn)
    {
        const string tableSql = @"
        CREATE TABLE IF NOT EXISTS Currencies (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Name VARCHAR(255) NOT NULL,
            Symbol VARCHAR(255) NOT NULL
        );";

        await conn.ExecuteAsync(tableSql);
        _logger.LogInformation("Table 'Currency' ensured.");
    }

    private async Task CreateSystemSettingsTableAsync(MySqlConnection conn)
    {
        const string tableSql = @"
        CREATE TABLE IF NOT EXISTS SystemSettings (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            TimezoneId INT NOT NULL,
            CurrencyId INT NOT NULL,
            FOREIGN KEY (TimezoneId) REFERENCES Timezones(Id),
            FOREIGN KEY (CurrencyId) REFERENCES Currencies(Id)
        );";

        await conn.ExecuteAsync(tableSql);
        _logger.LogInformation("Table 'SystemSettings' ensured.");
    }
}