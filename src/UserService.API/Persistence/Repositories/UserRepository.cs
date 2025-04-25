using Dapper;
using MySqlConnector;
using UserService.API.Abstraction;
using UserService.API.Exceptions;
using UserService.API.Persistence.Entities;

namespace UserService.API.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); 

        using var connection = await _context.CreateConnectionAsync();
        const string query = @"
            SELECT u.*, 
                   r.Id AS Role_Id, r.Name AS Role_Name, r.Description AS Role_Description 
            FROM Users u
            JOIN Roles r ON u.RoleId = r.Id
            ORDER BY u.Id
            LIMIT @PageSize OFFSET @Offset";

        var offset = (page - 1) * pageSize;

        return await connection.QueryAsync<User, Role, User>(
            query,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new {PageSize = pageSize, Offset = offset},
            splitOn: "Role_Id"
        );
    }

    public async Task<int> GetTotalUserCountAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); 

        using var connection = await _context.CreateConnectionAsync();
        const string query = "SELECT COUNT(*) FROM Users;";
        
        return await connection.ExecuteScalarAsync<int>(query);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = await _context.CreateConnectionAsync();
        const string query = @"
        SELECT Id, RoleId, Username, FirstName, LastName, Email, State, 
               PhoneNumber, Sex, Weight, Height, DateOfBirth, CreatedOn, UpdatedOn
        FROM Users
        WHERE Id = @Id;";

        return await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
    }

    public async Task<int?> AddAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
            INSERT INTO Users (RoleId, Username, FirstName, LastName, Email, State, PhoneNumber, Sex, Weight, Height, DateOfBirth, CreatedOn, UpdatedOn) 
            VALUES (@RoleId, @Username, @FirstName, @LastName, @Email, @State, @PhoneNumber, @Sex, @Weight, @Height, @DateOfBirth, NOW(), NOW());
            SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int>(query, user);
        }
        catch (MySqlException ex)
        {
            throw new DuplicateEntryException("User already exists.", ex);
        }
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); 

        using var connection = await _context.CreateConnectionAsync();
        const string query = @"
        UPDATE Users 
        SET RoleId = @RoleId, Username = @Username, FirstName = @FirstName, LastName = @LastName, Email = @Email, 
             State = @State, PhoneNumber = @PhoneNumber, Sex = @Sex, 
            Weight = @Weight, Height = @Height, DateOfBirth = @DateOfBirth, UpdatedOn = NOW()
        WHERE Id = @Id;";

        int rowsAffected = await connection.ExecuteAsync(query, user);

        return rowsAffected > 0; 
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = await _context.CreateConnectionAsync();
        const string query = "DELETE FROM Users WHERE Id = @Id;";
        
        int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });

        return rowsAffected > 0; 
    }
}