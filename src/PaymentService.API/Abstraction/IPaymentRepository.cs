using PaymentService.API.Persistence.Entities.DB.Models;

namespace PaymentService.API.Abstraction
{
    public interface IPaymentRepository
    {
        Task<int?> AddAsync(Payment payment, CancellationToken cancellationToken);
        Task<int?> AddAsyncCredits(Payment payment, CancellationToken cancellationToken);
        Task<int?> ChangeStatus(Payment payment, CancellationToken cancellationToken);
        Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<UserCredit?> GetBalanceByIdAsync(int id, CancellationToken cancellationToken);
        Task UpdateCredits(UserCredit userCredit, CancellationToken cancellationToken);
        Task<int?> AddBalanceAsync(UserCredit balance, CancellationToken cancellationToken);
    }
}
