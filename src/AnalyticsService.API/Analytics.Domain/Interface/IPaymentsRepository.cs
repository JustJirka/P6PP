using Analytics.Domain.Entities;

namespace Analytics.Domain.Interface
{
    public interface IPaymentsRepository
    {
        Task<List<Payment>> GetAll();
        Task<Payment?> GetById(int id);
        Task Create(Payment payment);
        Task<Payment?> Delete(int id);
        Task SaveChanges();
    }
}