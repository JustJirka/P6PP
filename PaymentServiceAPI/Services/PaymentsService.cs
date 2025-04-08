using Microsoft.Extensions.Caching.Memory;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Persistence.Repositories;
using System.Text.RegularExpressions;

namespace PaymentService.API.Services;

public class PaymentService
{
    private readonly PaymentRepository _paymentRepository;
    private readonly IMemoryCache _cache;
    
    public PaymentService(PaymentRepository paymentRepository, IMemoryCache cache)
    {
        _paymentRepository = paymentRepository;
        _cache = cache;
    }
    
    public async Task<Payment?> GetPaymentByIdAsync(int id, CancellationToken cancellationToken)
    {
        string cacheKey = $"payment:{id}";

        if (!_cache.TryGetValue(cacheKey, out Payment? payment))
        {
            payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);

        }

        return payment;
    }

    public async Task<int?> CreatePayment(Payment payment, CancellationToken cancellationToken)
    {
        string cacheKey = $"payment:{payment.Id}";

        var newPayment = await _paymentRepository.AddAsync(payment, cancellationToken);

        return newPayment;
    }

    public async Task<int?> CreatePaymentCredits(Payment payment, CancellationToken cancellationToken)
    {
        string cacheKey = $"payment:{payment.Id}";

        var newPayment = await _paymentRepository.AddAsyncCredits(payment, cancellationToken);

        return newPayment;
    }

    public async Task<int?> CangeStatus(Payment payment, CancellationToken cancellationToken)
    {
        string cacheKey = $"role:{payment.Id}";

        var newRole = await _paymentRepository.AddAsync(payment, cancellationToken);

        return newRole;
    }
}