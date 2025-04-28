using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Analytics.Infrastructure.Data;
public class DatabaseInit
{
    private readonly string _connectionString;
    private readonly string _adminConnectionString;
    private readonly string _databaseName;
    private readonly ILogger<DatabaseInit> _logger;

    public DatabaseInit(IConfiguration configuration, ILogger<DatabaseInit> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Database connection string is missing in appsettings.json.");
        _logger = logger;

        var builder = new MySqlConnectionStringBuilder(_connectionString);
        _databaseName = builder.Database;

        builder.Database = "";  // This allows admin-level queries
        _adminConnectionString = builder.ToString();
    }

    public async Task InitializeDatabase()
    {
        try
        {
            _logger.LogInformation("Checking if database '{Database}' exists...", _databaseName);

            await using var adminConnection = new MySqlConnection(_adminConnectionString);
            await adminConnection.OpenAsync();

            var existsQuery = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @DatabaseName;";
            var databaseExists = await adminConnection.ExecuteScalarAsync<string>(existsQuery, new { DatabaseName = _databaseName });

            if (databaseExists == null)
            {
                _logger.LogInformation("Database '{Database}' does not exist. Creating now...", _databaseName);
                await adminConnection.ExecuteAsync($"CREATE DATABASE `{_databaseName}`;");
                _logger.LogInformation("Database '{Database}' created successfully.", _databaseName);
            }
            else
            {
                _logger.LogInformation("Database '{Database}' already exists.", _databaseName);
            }

            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    RoleId INT NOT NULL,
                    State VARCHAR(20) NOT NULL,
                    Sex VARCHAR(10) NULL,
                    Weight DECIMAL(5, 2) NULL,
                    Height DECIMAL(5, 2) NULL,
                    DateOfBirth DATETIME NULL,
                    CreatedOn DATETIME DEFAULT CURRENT_TIMESTAMP,
                    UpdatedOn DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
                );";

            await connection.ExecuteAsync(createTableQuery);
            _logger.LogInformation("'Users' table checked/created successfully.");

            const string createBookingsTableQuery = @"
                CREATE TABLE IF NOT EXISTS Bookings (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    BookingDate DATETIME NOT NULL,
                    Status INT NOT NULL,
                    UserId INT NOT NULL,
                    ServiceId INT NOT NULL
                );";

            await connection.ExecuteAsync(createBookingsTableQuery);
            _logger.LogInformation("'Bookings' table checked/created successfully.");

        }
        catch (Exception ex)
        {
            _logger.LogError("Error initializing database: {Message}", ex.Message);
            throw;
        }
    }
}