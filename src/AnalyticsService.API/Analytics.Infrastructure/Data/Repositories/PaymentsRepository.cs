using Dapper;
using Analytics.Domain.Interface;
using Analytics.Domain.Entities;

namespace Analytics.Infrastructure.Data.Repositories
{
    public class PaymentsRepository(DapperContext context) : IPaymentsRepository
    {
        public async Task<List<Payment>> GetAll()
        {
            await using var connection = await context.CreateConnectionAsync();
            const string sql = @"
                SELECT 
                    PaymentID as Id, UserId, RoleId, Price, CreditAmount, 
                    Status, TransactionType, CreatedAt
                FROM Payments;";

            var payments = await connection.QueryAsync<Payment>(sql);
            return payments.AsList();
        }

        public async Task<Payment?> GetById(int id)
        {
            await using var connection = await context.CreateConnectionAsync();
            const string sql = @"
                SELECT 
                    PaymentID as Id, UserId, RoleId, Price, CreditAmount, 
                    Status, TransactionType, CreatedAt
                FROM Payments
                WHERE PaymentID = @Id;";

            return await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { Id = id });
        }

        public async Task Create(Payment payment)
        {
            await using var connection = await context.CreateConnectionAsync();
            const string sql = @"
                INSERT INTO Payments 
                    (UserId, RoleId, Price, CreditAmount, Status, TransactionType, CreatedAt)
                VALUES 
                    (@UserId, @RoleId, @Price, @CreditAmount, @Status, @TransactionType, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            // ExecuteScalar returns the auto-generated ID
            var id = await connection.ExecuteScalarAsync<int>(sql, new
            {
                payment.UserId,
                payment.RoleId,
                payment.Price,
                payment.CreditAmount,
                payment.Status,
                payment.TransactionType,
                payment.CreatedAt
            });

            payment.Id = id;
        }

        public async Task<Payment?> Delete(int id)
        {
            // First, retrieve the entity so we can return it after deletion
            var existing = await GetById(id);
            if (existing == null)
                return null;

            await using var connection = await context.CreateConnectionAsync();
            const string sql = "DELETE FROM Payments WHERE PaymentID = @Id;";
            await connection.ExecuteAsync(sql, new { Id = id });

            return existing;
        }

        public Task SaveChanges() => Task.CompletedTask;
    }
}