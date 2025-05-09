using NotificationService.API.Persistence.Entities;

namespace NotificationService.API.Services
{
    public interface IUserAppService
    {
        Task<User?> GetUserByIdAsync(int id);
    }
}
