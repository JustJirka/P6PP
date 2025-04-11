using AdminSettings.Persistence.Interface;

namespace AdminSettings.Services;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    
    public UserServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<bool> UserExistsAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"/api/user/{userId}");
        return response.IsSuccessStatusCode;

    }
}