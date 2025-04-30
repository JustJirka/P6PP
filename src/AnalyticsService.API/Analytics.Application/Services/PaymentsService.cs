using Analytics.Application.DTOs;
using Analytics.Application.Services.Interface;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;

namespace Analytics.Application.Services
{
    public class PaymentsService(IPaymentsRepository paymentRepository) : IPaymentsService
    {
        public async Task<List<Payment>> GetAllPayments()
        {
            return await paymentRepository.GetAll();
        }

        public async Task<Payment?> GetPaymentById(int id)
        {
            return await paymentRepository.GetById(id);
        }

        public async Task<Payment> CreatePayment(PaymentDto paymentDto)
        {
            var payment = new Payment
            {
                Id = paymentDto.id,
                UserId = paymentDto.userId,
                RoleId = paymentDto.roleId,
                Price = paymentDto.price,
                CreditAmount = paymentDto.creditAmount,
                Status = paymentDto.status,
                TransactionType = paymentDto.transactionType,
                CreatedAt = DateTime.Parse(paymentDto.createdAt)
            };

            await paymentRepository.Create(payment);
            return payment;
        }

        public async Task<Payment?> DeletePayment(int id)
        {
            return await paymentRepository.Delete(id);
        }
    }
}