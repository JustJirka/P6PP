﻿
using Dapper;
using Microsoft.EntityFrameworkCore;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentsService.API.Persistence;

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
                INSERT INTO Payment (UserId, Price, Status, CreatedAt, TransactionType)
                VALUES (@UserId, @Price, @Status, @CreatedAt, 'reservation');
                SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int?>(query, new
            {
                payment.UserId,
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
                INSERT INTO Payment (UserId, Price, Status, CreatedAt, TransactionType)
                VALUES (@UserId, @CreditAmount, @Status, @CreatedAt, 'credit' );
                SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int?>(query, new
            {
                payment.UserId,
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
        SELECT PaymentID, UserId,RoleId,ReceiverBankNumber,GiverBankNumber,Price,CreditAmount,IsValid,TransactionType
        FROM Payment
        WHERE UserId = @Id;";

            return await connection.QueryFirstOrDefaultAsync<Payment>(query, new { Id = id });
        }

        internal async Task<int> UpdateStatusByPaymentIdAsync(ulong paymentId, string status, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"  
               UPDATE Payment  
               SET Status = @Status  
               WHERE PaymentID = @PaymentID;";

            return await connection.ExecuteAsync(query, new
            {
                PaymentID = paymentId,
                Status = status
            });
        }


    }
}
