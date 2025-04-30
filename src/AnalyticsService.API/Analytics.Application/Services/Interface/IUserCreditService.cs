using Analytics.Application.DTOs;
using Analytics.Domain.Entities;

namespace Analytics.Application.Services.Interface
{
    public interface IUserCreditService
    {
        Task<List<UserCredit>> GetAllUserCredits();
        Task<UserCredit?> GetUserCreditById(int id);
        Task<UserCredit> CreateUserCredit(UserCreditDto userCredit);
        Task<UserCredit?> DeleteUserCredit(int id);
    }
}