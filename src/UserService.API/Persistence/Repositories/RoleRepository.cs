using Dapper;
using UserService.API.Abstraction;
using UserService.API.Persistence.Entities;

namespace UserService.API.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly DapperContext _context;

    public RoleRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await using var connection = await _context.CreateConnectionAsync();
        const string query = "SELECT * FROM Roles;";
        return await connection.QueryAsync<Role>(query);
    }

    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        using var connection = await _context.CreateConnectionAsync();
        const string query = "SELECT * FROM Roles WHERE Id = @Id;";
        return await connection.QuerySingleOrDefaultAsync<Role>(query, new { Id = id });
    }

    public async Task<int?> AddAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = await _context.CreateConnectionAsync();
        const string query = @"
        INSERT INTO Roles (Name, Description) 
        VALUES (@Name, @Description);
        SELECT LAST_INSERT_ID();"; 

        return await connection.ExecuteScalarAsync<int>(query, role);
    }

    public async Task UpdateAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        using var connection = await _context.CreateConnectionAsync();
        const string query = @"
            UPDATE Roles 
            SET Name = @Name, Description = @Description
            WHERE Id = @Id;";
        await connection.ExecuteAsync(query, role);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        using var connection = await _context.CreateConnectionAsync();
        const string query = "DELETE FROM Roles WHERE Id = @Id;";
        await connection.ExecuteAsync(query, new { Id = id });
    }
}