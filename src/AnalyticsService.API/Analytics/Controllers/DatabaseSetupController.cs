using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Analytics.Infrastructure.Data;

namespace Analytics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseSetupController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly ILogger<DatabaseSetupController> _logger;

        public DatabaseSetupController(DapperContext context, ILogger<DatabaseSetupController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("create-tables")]
        public async Task<IActionResult> CreateTables()
        {
            using var connection = await _context.CreateConnectionAsync();
            
            try {
                // Create Services table
                await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS Services (
                        Id INT PRIMARY KEY AUTO_INCREMENT,
                        ServiceName VARCHAR(255) NOT NULL,
                        Start DATETIME NOT NULL,
                        End DATETIME NOT NULL,
                        Price DECIMAL(10,2) NOT NULL DEFAULT 0,
                        IsCancelled BOOLEAN NOT NULL DEFAULT FALSE,
                        TrainerId INT NOT NULL,
                        RoomId INT NOT NULL
                    );
                ");
                
                // Create Rooms table
                await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS Rooms (
                        Id INT PRIMARY KEY AUTO_INCREMENT,
                        Name VARCHAR(255) NOT NULL,
                        Capacity INT NOT NULL DEFAULT 0,
                        Status VARCHAR(50) NOT NULL DEFAULT 'Available'
                    );
                ");
                
                // Create ServiceUsers table
                await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS ServiceUsers (
                        ServiceId INT NOT NULL,
                        UserId INT NOT NULL,
                        PRIMARY KEY (ServiceId, UserId),
                        FOREIGN KEY (ServiceId) REFERENCES Services(Id) ON DELETE CASCADE
                    );
                ");

                _logger.LogInformation("Database tables created successfully");
                return Ok("Tables created successfully");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error creating database tables");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        
        [HttpPost("seed-data")]
        public async Task<IActionResult> SeedData()
        {
            using var connection = await _context.CreateConnectionAsync();
            
            try {
                // Create rooms
                await connection.ExecuteAsync(@"
                    INSERT IGNORE INTO Rooms (Id, Name, Capacity, Status)
                    VALUES 
                    (1, 'Fitness Studio A', 20, 'Available'),
                    (2, 'Yoga Studio', 15, 'Available'),
                    (3, 'Cardio Zone', 30, 'Available');
                ");
                
                // Create services for each booking in your table
                await connection.ExecuteAsync(@"
                    INSERT IGNORE INTO Services (Id, ServiceName, Start, End, Price, IsCancelled, TrainerId, RoomId)
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
                    FROM Bookings b;
                ");

                _logger.LogInformation("Seed data added successfully");
                return Ok("Seed data added successfully");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error seeding database");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}