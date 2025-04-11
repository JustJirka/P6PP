namespace AdminSettings.Persistence.Interface;

public interface IUserServiceClient
{
    Task<bool> UserExistsAsync(int userId);

    
}