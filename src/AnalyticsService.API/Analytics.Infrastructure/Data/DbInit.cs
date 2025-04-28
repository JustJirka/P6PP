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

            // Create Services table
            const string createServicesTableQuery = @"
                CREATE TABLE IF NOT EXISTS Services (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    ServiceName VARCHAR(255) NOT NULL,
                    Start DATETIME NOT NULL,
                    End DATETIME NOT NULL,
                    Price DECIMAL(10,2) NOT NULL DEFAULT 0,
                    IsCancelled BOOLEAN NOT NULL DEFAULT FALSE,
                    TrainerId INT NOT NULL,
                    RoomId INT NOT NULL
                );";

            await connection.ExecuteAsync(createServicesTableQuery);
            _logger.LogInformation("'Services' table checked/created successfully.");

            // Create Rooms table  
            const string createRoomsTableQuery = @"
                CREATE TABLE IF NOT EXISTS Rooms (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Name VARCHAR(255) NOT NULL,
                    Capacity INT NOT NULL DEFAULT 0,
                    Status VARCHAR(50) NOT NULL DEFAULT 'Available'
                );";

            await connection.ExecuteAsync(createRoomsTableQuery);
            _logger.LogInformation("'Rooms' table checked/created successfully.");

            // Create ServiceUsers table (many-to-many relationship)
            const string createServiceUsersTableQuery = @"
                CREATE TABLE IF NOT EXISTS ServiceUsers (
                    ServiceId INT NOT NULL,
                    UserId INT NOT NULL,
                    PRIMARY KEY (ServiceId, UserId),
                    FOREIGN KEY (ServiceId) REFERENCES Services(Id) ON DELETE CASCADE
                );";

            await connection.ExecuteAsync(createServiceUsersTableQuery);
            _logger.LogInformation("'ServiceUsers' table checked/created successfully.");

            // Add sample data if the tables are empty
            await SeedInitialData(connection);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error initializing database: {Message}", ex.Message);
            throw;
        }
    }

    private async Task SeedInitialData(MySqlConnection connection)
    {
        // Check if Rooms table is empty
        var roomCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Rooms;");
        if (roomCount == 0)
        {
            _logger.LogInformation("Seeding rooms data...");
            await connection.ExecuteAsync(@"
                INSERT INTO Rooms (Id, Name, Capacity, Status)
                VALUES 
                (1, 'Fitness Studio A', 20, 'Available'),
                (2, 'Yoga Studio', 15, 'Available'),
                (3, 'Cardio Zone', 30, 'Available');
            ");
        }

        // Check if Bookings table has entries but Services doesn't
        var bookingCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Bookings;");
        var serviceCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Services;");
        
        if (bookingCount > 0 && serviceCount == 0)
        {
            _logger.LogInformation("Creating services for existing bookings...");
            await connection.ExecuteAsync(@"
                INSERT INTO Services (Id, ServiceName, Start, End, Price, IsCancelled, TrainerId, RoomId)
                SELECT 
                    b.ServiceId,
                    CONCAT('Service ', b.ServiceId),
                    DATE_ADD(b.BookingDate, INTERVAL -1 HOUR),
                    b.BookingDate,
                    100.00,
                    0,
                    1, -- Default trainer ID
                    CASE 
                        WHEN b.ServiceId % 3 = 0 THEN 3
                        WHEN b.ServiceId % 3 = 1 THEN 1
                        ELSE 2
                    END -- Assign rooms based on ServiceId
                FROM Bookings b
                WHERE NOT EXISTS (SELECT 1 FROM Services s WHERE s.Id = b.ServiceId);
            ");
        }
    }
}