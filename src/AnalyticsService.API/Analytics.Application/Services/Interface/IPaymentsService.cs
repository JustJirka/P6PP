using Analytics.Application.DTOs;
using Analytics.Domain.Entities;

namespace Analytics.Application.Services.Interface
{
    public interface IPaymentsService
    {
        Task<List<Payment>> GetAllPayments();
        Task<Payment?> GetPaymentById(int id);
        Task<Payment> CreatePayment(PaymentDto payment);
        Task<Payment?> DeletePayment(int id);
    }
}