using System.Data;
using MySqlConnector;
using Polly;
using Polly.Retry;
using Microsoft.Extensions.Configuration;

namespace Analytics.Infrastructure.Data;
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

    public async Task<MySqlConnection> CreateConnectionAsync()
    {
        var connection = new MySqlConnection(_connectionString);
        await _retryPolicy.ExecuteAsync(() => connection.OpenAsync());
        return connection;
    }
}