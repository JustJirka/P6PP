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


    public async Task<string> CreateBillAsync(int id, CancellationToken cancellationToken)
    {
        if (!_cache.TryGetValue($"payment:{id}", out Payment? payment))
        {
            payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);
        }

        if (payment == null)
        {
            throw new Exception("Payment not found.");
        }

        // Ask user for the folder to save the file
        string folderPath = string.Empty;
        using (var folderDialog = new FolderBrowserDialog())
        {
            folderDialog.Description = "Select the folder to save the bill";
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderDialog.SelectedPath;
            }
            else
            {
                throw new Exception("No folder selected.");
            }
        }

        string invoiceFilePath = Path.Combine(folderPath, $"Bill_{id}.pdf");

        using (FileStream fs = new FileStream(invoiceFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            document.Open();

            // Add title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            document.Add(new Paragraph("Bill", titleFont));
            document.Add(new Paragraph("\n"));

            // Add payment details
            var textFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            document.Add(new Paragraph($"UserID: {payment.UserId}", textFont));
            document.Add(new Paragraph($"PaymentID: {payment.PaymentID}", textFont));
            if (payment.TransactionType == "reservation")
            {
                document.Add(new Paragraph($"Price: {payment.Price} CZK", textFont));
            }
            else
            {
                document.Add(new Paragraph($"Credit Amount: {payment.CreditAmount} credits", textFont));
            }
            document.Add(new Paragraph($"Price: {payment.Price} CZK", textFont));
            document.Add(new Paragraph($"Date: {payment.CreatedAt:dd-MM-yyyy}", textFont));

            document.Close();
            writer.Close();
        }

        return "Success";
    }
}