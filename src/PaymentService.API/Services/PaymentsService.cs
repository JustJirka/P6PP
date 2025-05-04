using Microsoft.Extensions.Caching.Memory;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Persistence.Repositories;
using System.Text.RegularExpressions;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

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

    public async Task<int?> ChangeStatus(Payment payment, CancellationToken cancellationToken)
    {
        string cacheKey = $"payment:{payment.PaymentID}";

        var newRole = await _paymentRepository.ChangeStatus(payment, cancellationToken);

        return newRole;
    }

    public async Task<Payment?> GetTransactionType(int id, CancellationToken cancellationToken)
    {
        string cacheKey = $"payment:{id}";
        if (!_cache.TryGetValue(cacheKey, out Payment? payment))
        {
            payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);
            if (payment != null)
            {
                var regex = new Regex(@"^(credit|reservation)$");
                if (regex.IsMatch(payment.TransactionType))
                {
                    return payment;
                }
            }
        }
        return null;

    }

    public async Task<string> UpdateCreditsReservation(long userId, long Price, CancellationToken cancellationToken)
    {
        string cacheKey = $"user:{userId}";
        if (!_cache.TryGetValue(cacheKey, out UserCredit? payment))
        {
            payment = await _paymentRepository.GetBalanceByIdAsync((int)userId, cancellationToken);
            if (payment != null && payment.CreditBalance >= Price)
            {
                payment.CreditBalance -= Price;

               await _paymentRepository.UpdateCredits(payment, cancellationToken);

                return "Success";

            }
        }
        else if (payment != null && payment.CreditBalance >= Price)
        {
            payment.CreditBalance -= Price;

            await _paymentRepository.UpdateCredits(payment, cancellationToken);

            return "Success";

        }
        ;


        return "Failed";
    }

    public async Task<string> UpdateCredits(long userId, long CreditAmount, CancellationToken cancellationToken)
    {
        string cacheKey = $"user:{userId}";
        if (!_cache.TryGetValue(cacheKey, out UserCredit? payment))
        {
            payment = await _paymentRepository.GetBalanceByIdAsync((int)userId, cancellationToken);
            if (payment != null)
            {
                payment.CreditBalance += CreditAmount;

                await _paymentRepository.UpdateCredits(payment, cancellationToken);

                return "Success";

            }
        }
        else if (payment != null )
        {
            payment.CreditBalance += CreditAmount;

            await _paymentRepository.UpdateCredits(payment, cancellationToken);

            return "Success";

        }
        ;


        return "Failed";
    }

    public async Task<UserCredit?> GetBalanceByIdAsync(int userId, CancellationToken cancellationToken)
    {
        string cacheKey = $"user:{userId}";

        if (!_cache.TryGetValue(cacheKey, out UserCredit? payment))
        {
            payment = await _paymentRepository.GetBalanceByIdAsync(userId, cancellationToken);

        }

        return payment;
    }

    public async Task<int?> CreateBalanceAsync(UserCredit balance, CancellationToken cancellationToken)
    {
        string cacheKey = $"user:{balance.UserId}";

        var newPayment = await _paymentRepository.AddBalanceAsync(balance, cancellationToken);

        return newPayment;
    }
    public async Task<Payment> CreateBillAsync(int id, CancellationToken cancellationToken)
    {
        string cacheKey = $"payment:{id}";

        if (!_cache.TryGetValue(cacheKey, out Payment? payment))
        {
            payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);

        }

        if (payment == null)
        {
            throw new Exception("Payment not found.");
        }

        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string invoiceFilePath = Path.Combine(folderPath, $"Bill_{id}.txt");

        using (StreamWriter writer = new StreamWriter(invoiceFilePath, false))
        {
            // Add title
            writer.WriteLine("Bill");
            writer.WriteLine();

            // Add payment details
            writer.WriteLine($"UserID: {payment.UserId}");
            writer.WriteLine($"PaymentID: {payment.PaymentID}");
            if (payment.TransactionType == "reservation")
            {
                writer.WriteLine($"Price: {payment.Price} CZK");
            }
            else
            {
                writer.WriteLine($"Credit Amount: {payment.CreditAmount} credits");
            }
            writer.WriteLine($"Date: {payment.CreatedAt:dd-MM-yyyy}");
        }

        return payment;
    }
}