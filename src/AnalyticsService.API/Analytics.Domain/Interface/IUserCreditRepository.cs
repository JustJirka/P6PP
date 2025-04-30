using Analytics.Domain.Entities;

namespace Analytics.Domain.Interface
{
    public interface IUserCreditRepository
    {
        Task<List<UserCredit>> GetAll();
        Task<UserCredit?> GetById(int id);
        Task Create(UserCredit userCredit);
        Task<UserCredit?> Delete(int id);
        Task SaveChanges();
    }
}