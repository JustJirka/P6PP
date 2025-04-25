using System.Net.Mail;
using NotificationService.API.Persistence.Entities;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared;
using ReservationSystem.Shared.Results;
using NotificationService.API.Logging;

namespace NotificationService.API.Services
{
    public class UserAppService
    {
        public record GetUserRespond(User User);

        private readonly NetworkHttpClient _httpClient;

        public UserAppService(NetworkHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            ApiResult<GetUserRespond>? response = null;
            try
            {
                response = await _httpClient.GetAsync<GetUserRespond>(ServiceEndpoints.UserService.GetUserById(id));
            }
            catch (Exception e)
            {
                // Logování chyby při získávání uživatele
                await FileLogger.LogException(e, $"Error while fetching user with ID {id}");
                return null;
            }
            return response.Data?.User;
        }
    }
}
