using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ReservationSystem.Shared;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared.Results;
using AuthService.API.Data;
using AuthService.API.DTO;
using AuthService.API.Models;
using AuthService.API.Interfaces;

namespace AuthService.API.Services;

public class UserAuthService : IUserAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly NetworkHttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly AuthDbContext _dbContext;

    public UserAuthService(UserManager<ApplicationUser> userManager, NetworkHttpClient httpClient,
        IConfiguration configuration, AuthDbContext dbContext)
    {
        _userManager = userManager;
        _httpClient = httpClient;
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public async Task<ApiResult<object>> RegisterAsync(RegisterModel model)
    {
        var existingEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingEmail != null)
            return new ApiResult<object>(null, false, "User with this email already exists.");

        var existingUsername = await _userManager.FindByNameAsync(model.UserName);
        if (existingUsername != null)
            return new ApiResult<object>(null, false, "User with this username already exists.");

        var url = ServiceEndpoints.UserService.CreateUser;
        var newUser = new
        {
            Username = model.UserName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email.ToLower(),
        };

        var response = await _httpClient.PostAsync<object, object>(url, newUser, CancellationToken.None);

        if (!response.Success)
            return new ApiResult<object>(null, false, response.Message);

        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.UserName,
            UserId = int.Parse(response.Data.ToString())
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return new ApiResult<object>(result.Errors, false, result.Errors.ToString());

        var verifyToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(verifyToken));
        Console.WriteLine("Encoded token for email verification: " + encodedToken);

        var verificationUrl = ServiceEndpoints.NotificationService.SendVerificationEmail;
        var body = new
        {
            Id = user.UserId,
            Token = encodedToken,
        };

        var verificationResult =
            await _httpClient.PostAsync<object, object>(verificationUrl, body, CancellationToken.None);

        if (!verificationResult.Success)
        {
            Console.WriteLine("Failed to send verification email.");
        }

        Console.WriteLine("Preparing to send registration email...");
        var registrationUrl = ServiceEndpoints.NotificationService.SendRegistrationEmail(user.UserId);
        var message = await _httpClient.GetAsync<object>(registrationUrl);

        if (!message.Success)
        {
            Console.WriteLine("Failed to send registration email.");
        }
        else
        {
            Console.WriteLine("Registration email sent successfully.");
        }

        return new ApiResult<object>(new { Id = user.UserId });
    }

    public async Task<ApiResult<string>> LoginAsync(LoginModel model)
    {
        ApplicationUser user = null;

        if (!string.IsNullOrEmpty(model.UsernameOrEmail))
        {
            user = await _userManager.FindByEmailAsync(model.UsernameOrEmail)
                   ?? await _userManager.FindByNameAsync(model.UsernameOrEmail);
        }

        if (user is null)
            return new ApiResult<string>(null, false, "Invalid username/email or password.");

        if (!user.EmailConfirmed)
            return new ApiResult<string>(null, false, "Email not verified.");

        var userUrl = ServiceEndpoints.UserService.GetUserById(user.UserId);
        var userResponse = await _httpClient.GetAsync<UserResponse>(userUrl);

        if (!userResponse.Success)
            return new ApiResult<string>(null, false, userResponse.Message);

        if (userResponse.Data?.User.State == "Deactivated")
        {
            return new ApiResult<string>(null, false, "User had deactivated account.");
        }

        var result = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!result)
            return new ApiResult<string>(null, false, "Invalid username/email or password.");

        var token = GenerateJwtToken(user);
        return new ApiResult<string>(token);
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var secretKey = _configuration["JWT_SECRET_KEY"];
        var issuer = _configuration["JWT_ISSUER"];
        var audience = _configuration["JWT_AUDIENCE"];

        var claims = new List<Claim>
        {
            new Claim("userid", user.UserId.ToString()),
            new Claim("username", user.UserName!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<ApiResult<object>> RequestPasswordChangeAsync(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "userid");

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            return new ApiResult<object>(null, false, "Token does not contain valid user ID.");

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
            return new ApiResult<object>(null, false, "User not found.");

        var notificationUrl = ServiceEndpoints.NotificationService.SendPasswordResetEmail;
        var rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
        var resetUrl = $"://mojeapp.cz/reset-password?userId={user.UserId}&token={encodedToken}";

        var body = new
        {
            Id = user.UserId,
            Token = encodedToken
        };

        var response = await _httpClient.PostAsync<object, object>(notificationUrl, body, CancellationToken.None);

        if (!response.Success)
            return new ApiResult<object>(null, false, response.Message);

        return new ApiResult<object>(null);
    }

    public async Task<ApiResult<object>> ChangePasswordAsync(HttpContext httpContext, ChangePasswordModel model)
    {
        var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "userid");

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            return new ApiResult<object>(null, false, "Token does not contain valid user ID.");

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
            return new ApiResult<object>(null, false, "User not found.");

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

        if (!result.Succeeded)
            return new ApiResult<object>(result.Errors, false, "Password changed failed.");

        return new ApiResult<object>(new { UserId = user.UserId, Email = user.Email }, true,
            "Password changed successfully.");
    }

    public async Task<ApiResult<object>> RequestPasswordResetAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return new ApiResult<object>(null, false, "Email is required.");
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return new ApiResult<object>(null, false, "User with this email does not exist.");

        var notificationUrl = ServiceEndpoints.NotificationService.SendPasswordResetEmail;

        var rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
        var body = new
        {
            Id = user.UserId,
            Token = encodedToken
        };

        var response = await _httpClient.PostAsync<object, object>(notificationUrl, body, CancellationToken.None);

        if (!response.Success)
            return new ApiResult<object>(null, false, response.Message);

        Console.WriteLine($"token: {encodedToken}");
        return new ApiResult<object>(null);
    }

    public async Task<ApiResult<object>> ResetPasswordAsync(ResetPasswordModel model)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == model.userId);
        if (user is null)
            return new ApiResult<object>(null, false, "User not found.");

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
        if (!result.Succeeded)
            return new ApiResult<object>(result.Errors, false, "Password reset failed.");

        return new ApiResult<object>(new { UserId = user.UserId }, true, "Password reset successfully.");
    }

    public async Task<ApiResult<object>> IsVerifiedAsync(int userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user is null)
            return new ApiResult<object>(null, false, "User not found.");

        var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (!isEmailConfirmed)
            return new ApiResult<object>(new { verified = false }, false, "Email not verified.");

        return new ApiResult<object>(new { verified = true }, true, "User is verified.");
    }

    public async Task<ApiResult<object>> VerifyEmailAsync(int userId, string token)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
            return new ApiResult<object>(null, false, "User not found.");

        try
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
                return new ApiResult<object>(null, true, "Email verified successfully.");

            return new ApiResult<object>(null, false, "Invalid or expired token.");
        }
        catch (FormatException)
        {
            return new ApiResult<object>(null, false, "Token format is invalid.");
        }
        catch (Exception)
        {
            return new ApiResult<object>(null, false, "An unexpected error occurred.");
        }
    }

    public async Task<ApiResult<object>> LogoutAsync(HttpContext httpContext, string token)
    {
        var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "userid");
        var usernameClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "username");

        if (userIdClaim is null || usernameClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return new ApiResult<object>(new
            {
                message = "Token is invalid or claims are missing",
                claims = httpContext.User.Claims.Select(c => new { c.Type, c.Value })
            }, false);
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
        {
            return new ApiResult<object>(new { message = "User not found in database." }, false);
        }

        var expClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "exp");

        if (expClaim is null || !long.TryParse(expClaim.Value, out var expUnix))
        {
            return new ApiResult<object>(new { message = "Missing or invalid exp claim" }, false);
        }

        var expirationDateUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

        var tokenBlackList = new TokenBlackList
        {
            UserId = user.Id, // string (Identity GUID)
            UserNumericId = user.UserId, // int (Vlastní ID)
            Token = token,
            ExpirationDate = expirationDateUtc
        };

        _dbContext.TokenBlackLists.Add(tokenBlackList);
        await _dbContext.SaveChangesAsync();

        return new ApiResult<object>(new
        {
            message = "User logged out successfully.",
            userid = user.UserId,
            identityId = user.Id,
            username = user.UserName,
            claims = httpContext.User.Claims.Select(c => new { c.Type, c.Value })
        }, true);
    }

    public async Task<ApiResult<bool>> DeleteUserAsync(int userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
        {
            return new ApiResult<bool>(false, false, "User not found.");
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new ApiResult<bool>(false, false, $"Failed to delete user: {errors}");
        }

        return new ApiResult<bool>(true, true, "User deleted successfully.");
    }
}