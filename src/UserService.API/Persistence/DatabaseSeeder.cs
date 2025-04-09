using Dapper;

namespace UserService.API.Persistence;

public class DatabaseSeeder
{
    private readonly DapperContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(DapperContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await using var connection = await _context.CreateConnectionAsync();

        _logger.LogInformation("Seeding roles...");

        // ✅ Insert roles only if they do not already exist
        const string insertRolesQuery = @"
            INSERT INTO Roles (Name, Description)
            SELECT * FROM (SELECT 'Admin', 'Administrator Role') AS tmp
            WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
            UNION
            SELECT * FROM (SELECT 'User', 'Standard User Role') AS tmp
            WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'User')
            UNION
            SELECT * FROM (SELECT 'Trainer', 'Trainer Role') AS tmp
            WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Manager');";

        await connection.ExecuteAsync(insertRolesQuery);

        _logger.LogInformation("Roles seeded successfully.");
    }
}