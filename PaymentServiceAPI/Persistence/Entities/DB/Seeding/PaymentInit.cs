using PaymentService.API.Persistence.Entities.DB.Models;

namespace PaymentService.API.Persistence.Entities.DB.Seeding;

public class PaymentInit
{
    public IList<Payment> GetPayments()
    {
        List<Payment> Payments = new List<Payment>();
        Payments.Add(new Payment
        {
            PaymentID = 1,
            UserId = 3,
            //RoleId = 1,
            //ReceiverBankNumber = "1234567890",
            //GiverBankNumber = "0987654321",
            CreditAmount = 250,
            Status = "completed",
            TransactionType = "credit"
        });
        Payments.Add(new Payment
        {
            PaymentID = 2,
            UserId = 2,
            //RoleId = 2, student
            //ReceiverBankNumber = "3214569033",
            //GiverBankNumber = "834234213",
            Price = 200,
            Status = "completed",
            TransactionType = "reservation"
        });
        Payments.Add(new Payment
        {
            PaymentID = 3,
            UserId = 3,
            //RoleId = 1,
            //ReceiverBankNumber = "9029349234",
            //GiverBankNumber = "3213933921",
            Price = 500,
            Status = "failed",
            TransactionType = "reservation"
        });

        return Payments;
    }
}
