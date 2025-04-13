using PaymentService.API.Persistence.Entities.DB.Models;

namespace PaymentService.API.Persistence.Entities.DB.Seeding
{
    public class UserCreditInit
    {
        public IList<UserCredit> GetUserCredit()
        {
            List<UserCredit> UserCredits = new List<UserCredit>();
            UserCredits.Add(new UserCredit
            {
                UserId = 1,
                RoleId = 1,
                CreditBalance = 0
            });
            UserCredits.Add(new UserCredit
            {
                UserId = 2,
                RoleId = 2,
                CreditBalance = 200
            });
            UserCredits.Add(new UserCredit
            {
                UserId = 3,
                RoleId = 3,
                CreditBalance = 250
            });
            return UserCredits;
        }
    }
}
