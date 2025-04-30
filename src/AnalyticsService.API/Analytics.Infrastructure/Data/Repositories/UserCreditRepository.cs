using Dapper;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;

namespace Analytics.Infrastructure.Data.Repositories
{
public class UserCreditRepository(DapperContext context) : IUserCreditRepository
{
    public async Task<List<UserCredit>> GetAll()
    {
        await using var connection = await context.CreateConnectionAsync();
        const string sql = @"
            SELECT 
                Id, UserId, RoleId, CreditBalance
            FROM UserCredits;";

        var credits = await connection.QueryAsync<UserCredit>(sql);
        return credits.AsList();
    }

    public async Task<UserCredit?> GetById(int id)
    {
        await using var connection = await context.CreateConnectionAsync();
        const string sql = @"
            SELECT 
                Id, UserId, RoleId, CreditBalance
            FROM UserCredits
            WHERE Id = @Id;";

        return await connection.QueryFirstOrDefaultAsync<UserCredit>(sql, new { Id = id });
    }

    public async Task Create(UserCredit userCredit)
    {
        await using var connection = await context.CreateConnectionAsync();
        const string sql = @"
            INSERT INTO UserCredits 
                (UserId, RoleId, CreditBalance)
            VALUES 
                (@UserId, @RoleId, @CreditBalance);
            SELECT LAST_INSERT_ID();";

        var id = await connection.ExecuteScalarAsync<int>(sql, new
        {
            userCredit.UserId,
            userCredit.RoleId,
            userCredit.CreditBalance
        });

        userCredit.Id = id;
    }

    public async Task<UserCredit?> Delete(int id)
    {
        var existing = await GetById(id);
        if (existing == null)
            return null;

        await using var connection = await context.CreateConnectionAsync();
        const string sql = "DELETE FROM UserCredits WHERE Id = @Id;";
        await connection.ExecuteAsync(sql, new { Id = id });

        return existing;
    }

    public Task SaveChanges() => Task.CompletedTask;
}
}