using Analytics.Application.DTOs;
using Analytics.Application.Services.Interface;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;

namespace Analytics.Application.Services
{
    public class UserCreditService(IUserCreditRepository userCreditRepository) : IUserCreditService
    {
        public async Task<List<UserCredit>> GetAllUserCredits()
        {
            return await userCreditRepository.GetAll();
        }

        public async Task<UserCredit?> GetUserCreditById(int id)
        {
            return await userCreditRepository.GetById(id);
        }

        public async Task<UserCredit> CreateUserCredit(UserCreditDto userCreditDto)
        {
            var userCredit = new UserCredit
            {
                Id = userCreditDto.id,
                UserId = userCreditDto.userId,
                RoleId = userCreditDto.roleId,
                CreditBalance = userCreditDto.creditBalance
            };

            await userCreditRepository.Create(userCredit);
            return userCredit;
        }

        public async Task<UserCredit?> DeleteUserCredit(int id)
        {
            return await userCreditRepository.Delete(id);
        }
    }
}