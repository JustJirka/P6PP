using AdminSettings.Persistence.Entities;
using Dapper;
using System.Data;
using System.Text;

namespace AdminSettings.Persistence.Repository;

public class AuditLogRepository
{
    private readonly DapperContext _context;

    public AuditLogRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync(int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate)
    {
        using var connection = await _context.CreateConnectionAsync();

        var query = new StringBuilder("SELECT * FROM AuditLogs WHERE 1=1");

        if (fromDate.HasValue)
            query.Append(" AND TimeStamp >= @FromDate");

        if (toDate.HasValue)
            query.Append(" AND TimeStamp <= @ToDate");

        query.Append(" ORDER BY TimeStamp DESC LIMIT @Offset, @PageSize;");

        return await connection.QueryAsync<AuditLog>(query.ToString(), new
        {
            FromDate = fromDate,
            ToDate = toDate,
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        });
    }


    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate)
    {
        using var connection = await _context.CreateConnectionAsync();

        var query = new StringBuilder("SELECT * FROM AuditLogs WHERE UserId = @UserId");

        if (fromDate.HasValue)
            query.Append(" AND TimeStamp >= @FromDate");

        if (toDate.HasValue)
            query.Append(" AND TimeStamp <= @ToDate");

        query.Append(" ORDER BY TimeStamp DESC LIMIT @Offset, @PageSize;");

        return await connection.QueryAsync<AuditLog>(query.ToString(), new
        {
            UserId = userId,
            FromDate = fromDate,
            ToDate = toDate,
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        });
    }


    public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action)
    {
        using var connection = await _context.CreateConnectionAsync();
        const string query = "SELECT * FROM AuditLogs WHERE Action = @Action;";
        return await connection.QueryAsync<AuditLog>(query, new { Action = action });
    }

    public async Task<AuditLog?> GetByIdAsync(int id)
    {
        using var connection = await _context.CreateConnectionAsync();
        const string query = "SELECT * FROM AuditLogs WHERE Id = @Id;";
        return await connection.QuerySingleOrDefaultAsync<AuditLog>(query, new { Id = id });
    }

    public async Task<int> AddAsync(AuditLog log)
    {
        using var connection = await _context.CreateConnectionAsync();
        const string query = @"
            INSERT INTO AuditLogs (UserId, TimeStamp, Action)
            VALUES (@UserId, @TimeStamp, @Action);
            SELECT LAST_INSERT_ID();";
        return await connection.ExecuteScalarAsync<int>(query, log);
    }

    public async Task ArchiveAsync(int id)
    {
        using var connection = await _context.CreateConnectionAsync();
        const string query = "UPDATE AuditLogs SET Action = 'Archived' WHERE Id = @Id;";
        await connection.ExecuteAsync(query, new { Id = id });
    }
}
