using System.Data;
using MySqlConnector;
using Polly;
using Polly.Retry;

namespace AdminSettings.Persistence;

public class DapperContext
{
    private readonly string _connectionString;
    private readonly AsyncRetryPolicy _retryPolicy;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        _retryPolicy = Policy
            .Handle<MySqlException>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new MySqlConnection(_connectionString);
        await _retryPolicy.ExecuteAsync(() => connection.OpenAsync());
        return connection;
    }
}