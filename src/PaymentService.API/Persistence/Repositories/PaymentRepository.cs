
using Dapper;
using Microsoft.EntityFrameworkCore;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Persistence;

namespace PaymentService.API.Persistence.Repositories
{
    public class PaymentRepository
    {
        private readonly DapperContext _context;
        public PaymentRepository(DapperContext context)
        {
            _context = context;
        }
        internal async Task<int?> AddAsync(Payment payment, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
                INSERT INTO Payment (UserId, RoleId, Price, CreditAmount, Status, CreatedAt, TransactionType)
                VALUES (@UserId, @RoleId, @Price, 0, @Status, @CreatedAt, 'reservation');
                SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int?>(query, new
            {
                payment.UserId,
                payment.RoleId,
                payment.Price,
                payment.Status,
                CreatedAt = DateTime.UtcNow
            });
        }

        internal async Task<int?> AddAsyncCredits(Payment payment, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
                INSERT INTO Payment (UserId, RoleId, Price, CreditAmount, Status, CreatedAt, TransactionType)
                VALUES (@UserId, @RoleId, 0, @CreditAmount, @Status, @CreatedAt, 'credit' );
                SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int?>(query, new
            {
                payment.UserId,
                payment.RoleId,
                payment.CreditAmount,
                payment.Status,
                CreatedAt = DateTime.UtcNow
            });
        }

        internal async Task<int?> ChangeStatus(Payment payment, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"  
               UPDATE Payment  
               SET Status = @Status  
               WHERE PaymentID = @PaymentID;";

            return await connection.ExecuteAsync(query, new
            {
                PaymentID = payment.PaymentID,
                Status = payment.Status
            });
        }

        internal async Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
        SELECT PaymentID, UserId,RoleId,Price,CreditAmount,Status,TransactionType, CreatedAt
        FROM Payment
        WHERE PaymentId = @Id;";

            return await connection.QueryFirstOrDefaultAsync<Payment>(query, new { Id = id });
        }

        internal async Task<UserCredit?> GetBalanceByIdAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
        SELECT CreditBalance, RoleId, UserId
        FROM UserCredit
        WHERE UserId = @Id;";

            return await connection.QueryFirstOrDefaultAsync<UserCredit>(query, new { Id = id });
        }

        internal async Task UpdateCredits(UserCredit userCredit, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"  
               UPDATE UserCredit  
               SET CreditBalance = @CreditBalance  
               WHERE UserId = @UserId;";

            await connection.ExecuteAsync(query, new
            {
                UserId = userCredit.UserId,
                CreditBalance = userCredit.CreditBalance
            });
        }

        internal async Task<int?> AddBalanceAsync(UserCredit balance, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
                INSERT INTO UserCredit (UserId, RoleId, CreditBalance)
                VALUES (@UserId, @RoleId, @CreditBalance);
                SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int?>(query, new
            {
                balance.UserId,
                balance.RoleId,
                balance.CreditBalance
                
            });
        }


        //internal async Task<string?> FindTransactionTypeAsync(int paymentId, CancellationToken cancellationToken)
        //{
        //    cancellationToken.ThrowIfCancellationRequested();
        //    using var connection = await _context.CreateConnectionAsync();
        //    const string query = @"
        //        SELECT TransactionType
        //        FROM Payment
        //        WHERE PaymentID = @PaymentID;";

        //    return await connection.QueryFirstOrDefaultAsync<string?>(query, new { PaymentID = paymentId });
        //}



    }
}
