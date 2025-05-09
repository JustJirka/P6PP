using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Analytics.Application.DTOs;
using Analytics.Application.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Analytics.Application.Services
{
    public class DatabaseSyncService : IDatabaseSyncService
    {
        private readonly ILogger<DatabaseSyncService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public DatabaseSyncService(ILogger<DatabaseSyncService> logger,
                                   HttpClient httpClient,
                                   IUserService userService,
                                   IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task SyncDatabase()
        {
            try
            {
                _logger.LogInformation("Starting database synchronization at {Time}", DateTime.UtcNow);

                // Načtení hodnot z konfigurace
                string baseUrl = _configuration["ExternalServices:UserService:BaseUrl"];
                string port = _configuration["ExternalServices:UserService:Port"];
                string usersEndpoint = _configuration["ExternalServices:UserService:UsersEndpoint"];

                // Construct the URL.
                string requestUrl = $"{baseUrl}:{port}{usersEndpoint}";

                // Make the GET call to the external API.
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                // Read the JSON response.
                string dataJson = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Data fetched from external API: {Data}", dataJson);

                // Deserialize the JSON. Options to ignore case.
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserDataResponse>>(dataJson, options);

                if (apiResponse != null && apiResponse.success && apiResponse.data != null)
                {
                    // Iterate over each user DTO from the external API.
                    foreach (var userDto in apiResponse.data.users)
                    {
                        // Call your service to create (or upsert) the user.
                        // Your UserService.CreateUser(UserDto) method maps the DTO to a User entity.
                        await _userService.CreateUser(userDto);
                    }
                    _logger.LogInformation("Successfully synced {Count} users.", apiResponse.data.totalCount);
                }
                else
                {
                    _logger.LogWarning("API response indicates failure or missing data.");
                }

                _logger.LogInformation("Database synchronization completed successfully at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database synchronization");
                throw;  // Let Quartz capture the failure if needed.
            }
        }
    }
}
