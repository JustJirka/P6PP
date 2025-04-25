using ReservationSystem.Shared.Results;
using AuthService.API.DTO;

namespace AuthService.API.Interfaces
{
    public interface IUserAuthService
    {
        Task<ApiResult<object>> RegisterAsync(RegisterModel model);
        Task<ApiResult<string>> LoginAsync(LoginModel model);
        Task<ApiResult<object>> RequestPasswordChangeAsync(HttpContext httpContext);
        Task<ApiResult<object>> ChangePasswordAsync(HttpContext httpContext, ChangePasswordModel model);
        Task<ApiResult<object>> RequestPasswordResetAsync(string email);
        Task<ApiResult<object>> ResetPasswordAsync(ResetPasswordModel model);
        Task<ApiResult<object>> IsVerifiedAsync(int userId);
        Task<ApiResult<object>> VerifyEmailAsync(int userId, string token);
        Task<ApiResult<object>> LogoutAsync(HttpContext httpContext, string token);
        Task<ApiResult<bool>> DeleteUserAsync(int userId);
    }
}