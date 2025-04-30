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
            
            // Create Payments table
            const string createPaymentsTableQuery = @"
                CREATE TABLE IF NOT EXISTS Payments (
                    PaymentID INT AUTO_INCREMENT PRIMARY KEY,
                    UserId BIGINT NOT NULL,
                    RoleId BIGINT NOT NULL,
                    Price BIGINT NOT NULL,
                    CreditAmount BIGINT NOT NULL,
                    Status VARCHAR(50) NOT NULL,
                    TransactionType VARCHAR(50) NOT NULL,
                    CreatedAt DATETIME NOT NULL
                );";

            await connection.ExecuteAsync(createPaymentsTableQuery);
            _logger.LogInformation("'Payments' table checked/created successfully.");

            // Create UserCredits table
            const string createUserCreditsTableQuery = @"
                CREATE TABLE IF NOT EXISTS UserCredits (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    UserId BIGINT NOT NULL,
                    RoleId BIGINT NOT NULL,
                    CreditBalance BIGINT NOT NULL
                );";

            await connection.ExecuteAsync(createUserCreditsTableQuery);
            _logger.LogInformation("'UserCredits' table checked/created successfully.");

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
                (2, 'Yoga Room', 15, 'Available'),
                (3, 'Fitness Hall', 30, 'Available');
            ");
        }

        // Check if Services table is empty
        var serviceCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Services;");
        if (serviceCount == 0)
        {
            _logger.LogInformation("Seeding services data...");
            await connection.ExecuteAsync(@"
                INSERT INTO Services (Id, ServiceName, Start, End, Price, IsCancelled, TrainerId, RoomId)
                VALUES 
                (1, 'Basic Personal Training', '2025-04-15T10:00:00', '2025-04-15T11:00:00', 50.00, 0, 1, 1),
                (2, 'Advanced Personal Training', '2025-04-16T16:00:00', '2025-04-16T17:30:00', 80.00, 0, 1, 1),
                (3, 'Yoga Class', '2025-04-14T08:00:00', '2025-04-14T09:00:00', 30.00, 0, 5, 2),
                (4, 'Group Training', '2025-04-12T17:00:00', '2025-04-12T18:00:00', 25.00, 0, 1, 3),
                (5, 'Pilates Class', '2025-04-17T12:00:00', '2025-04-17T13:00:00', 35.00, 0, 5, 2),
                (6, 'HIIT Workout', '2025-04-13T18:00:00', '2025-04-13T19:00:00', 40.00, 0, 1, 3);
            ");
        }

        // Check if ServiceUsers table is empty
        var serviceUsersCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ServiceUsers;");
        if (serviceUsersCount == 0)
        {
            _logger.LogInformation("Seeding service users data...");
            await connection.ExecuteAsync(@"
                INSERT INTO ServiceUsers (ServiceId, UserId)
                VALUES 
                (1, 4),
                (2, 2), (2, 3),
                (3, 6), (3, 4), (3, 2),
                (4, 3),
                (5, 2), (5, 6),
                (6, 4), (6, 2), (6, 6), (6, 3);
            ");
        }

        // Check if Bookings table is empty
        var bookingCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Bookings;");
        if (bookingCount == 0)
        {
            _logger.LogInformation("Seeding bookings data...");
            await connection.ExecuteAsync(@"
                INSERT INTO Bookings (Id, BookingDate, Status, UserId, ServiceId)
                VALUES 
                (1, '2025-04-08T14:30:00', 0, 4, 1),
                (2, '2025-04-09T16:00:00', 1, 2, 1),
                (3, '2025-04-07T09:15:00', 0, 6, 2),
                (4, '2025-04-05T18:20:00', 2, 3, 3),
                (5, '2025-04-10T11:45:00', 0, 2, 4),
                (6, '2025-04-06T08:30:00', 0, 4, 5);
            ");
        }
        
        // Check if Payments table is empty
        var paymentCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Payments;");
        if (paymentCount == 0)
        {
            _logger.LogInformation("Seeding payments data...");
            await connection.ExecuteAsync(@"
                INSERT INTO Payments (UserId, RoleId, Price, CreditAmount, Status, TransactionType, CreatedAt)
                VALUES 
                (1, 1, 1000, 100, 'Completed', 'Purchase', '2025-04-19 21:28:17'),
                (2, 0, 2000, 200, 'Pending', 'Subscription', '2025-04-21 20:15:00'),
                (3, 0, 500, 50, 'Failed', 'Purchase', '2025-04-24 10:30:00'),
                (4, 0, 1500, 150, 'Completed', 'Purchase', '2025-04-12 15:45:20'),
                (5, 1, 3000, 300, 'Completed', 'Subscription', '2025-04-05 09:30:00'),
                (6, 0, 800, 80, 'Pending', 'Purchase', '2025-04-18 18:20:30');
            ");
        }

        // Check if UserCredits table is empty
        var userCreditCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM UserCredits;");
        if (userCreditCount == 0)
        {
            _logger.LogInformation("Seeding user credits data...");
            await connection.ExecuteAsync(@"
                INSERT INTO UserCredits (UserId, RoleId, CreditBalance)
                VALUES 
                (1, 1, 500),
                (2, 0, 1000),
                (3, 0, 200),
                (4, 0, 750),
                (5, 1, 1500),
                (6, 0, 400);
            ");
        }

        // Check if Users table is empty
        var userCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users;");
        if (userCount == 0)
        {
            _logger.LogInformation("Seeding users data...");
            await connection.ExecuteAsync(@"
                INSERT INTO Users (Id, RoleId, State, Sex, Weight, Height, DateOfBirth, CreatedOn, UpdatedOn)
                VALUES 
                (1, 1, 'active', 'Male', 75.0, 185.0, '2002-04-12', '2025-04-10 19:00:00', '2025-04-10 19:00:00'),
                (2, 0, 'active', 'Female', 62.0, 168.0, '1995-08-22', '2025-03-15 10:20:00', '2025-04-08 09:45:00'),
                (3, 0, 'inactive', 'Male', 90.0, 178.0, '1988-03-10', '2025-02-28 16:30:00', '2025-04-01 11:20:00'),
                (4, 0, 'active', 'Female', 58.0, 162.0, '2000-11-05', '2025-03-20 13:45:00', '2025-04-05 14:30:00'),
                (5, 1, 'active', 'Male', 82.0, 190.0, '1992-06-18', '2025-01-10 08:15:00', '2025-03-25 17:10:00'),
                (6, 0, 'active', 'Female', 65.0, 170.0, '1997-09-30', '2025-02-15 11:30:00', '2025-04-07 10:00:00');
            ");
        }
    }
}