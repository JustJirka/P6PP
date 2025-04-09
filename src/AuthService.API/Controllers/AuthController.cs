using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.API.Data;
using AuthService.API.DTO;
using AuthService.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservationSystem.Shared;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared.Results;

namespace AuthService.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly NetworkHttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly AuthDbContext _dbContext;

    public AuthController(UserManager<ApplicationUser> userManager, NetworkHttpClient httpClient,
        IConfiguration configuration, AuthDbContext dbContext)
    {
        _userManager = userManager;
        _httpClient = httpClient;
        _configuration = configuration;
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var existingEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingEmail != null)
            return BadRequest(new ApiResult<object>(null, false, "User with this email already exists."));

        var existingUsername = await _userManager.FindByNameAsync(model.UserName);
        if (existingUsername != null)
            return BadRequest(new ApiResult<object>(null, false, "User with this username already exists."));

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
            return BadRequest(new ApiResult<object>(null, false, response.Message));

        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.UserName,
            UserId = int.Parse(response.Data.ToString())
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(new ApiResult<object>(result.Errors, false, result.Errors.ToString()));

        var verifyToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(verifyToken));

        // TODO: Send email with the token to the user using notification service
        Console.WriteLine($"Email verification token for user {user.Email}:");
        Console.WriteLine(encodedToken);
        
        return Ok(new ApiResult<object>(new { Id = user.UserId }));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResult<object>(null, false, "Invalid data."));

        ApplicationUser user = null;

        if (!string.IsNullOrEmpty(model.UsernameOrEmail))
        {
            // No manual normalization here
            user = await _userManager.FindByEmailAsync(model.UsernameOrEmail)
                   ?? await _userManager.FindByNameAsync(model.UsernameOrEmail);
        }

        if (user == null)
            return BadRequest(new ApiResult<object>(null, false, "Invalid username/email or password."));

        var result = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!result)
            return BadRequest(new ApiResult<object>(null, false, "Invalid username/email or password."));

        var token = GenerateJwtToken(user);
        return Ok(new ApiResult<string>(token));
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var secretKey = _configuration["JWT_SECRET_KEY"];
        var issuer = _configuration["JWT_ISSUER"];
        var audience = _configuration["JWT_AUDIENCE"];

        var claims = new List<Claim>
        {
            new Claim("userid", user.UserId.ToString()), //Změněno na UserId a ToString()
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

    [Authorize]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userid");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new ApiResult<object>(null, false, "Token does not contain valid user ID."));

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return BadRequest(new ApiResult<object>(null, false, "User not found."));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

        if (!result.Succeeded)
            return BadRequest(new ApiResult<object>(result.Errors, false, "Password reset failed."));

        return Ok(new ApiResult<object>(
            new { UserId = user.UserId, Email = user.Email },
            true,
            "Password reset successfully."));
    }

    
    [HttpGet("isVerified/{userId}")]
    public async Task<IActionResult> IsVerified(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest(new ApiResult<object>(null, false, "User not found."));

        var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (!isEmailConfirmed)
            return Ok(new ApiResult<object>(new {verified = false }, false, "Email not verified."));

        return Ok(new ApiResult<object>(new { verified = true }, true, "User is verified."));
    }
    
    [HttpPost("verify-email/{userId}/{token}")]
    public async Task<IActionResult> VerifyEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new ApiResult<object>(null, false, "User not found."));

        try
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
                return Ok(new ApiResult<object>(null, true, "Email verified successfully."));

            return BadRequest(new ApiResult<object>(null, false, "Invalid or expired token."));
        }
        catch (FormatException)
        {
            return BadRequest(new ApiResult<object>(null, false, "Token format is invalid."));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResult<object>(null, false, "An unexpected error occurred."));
        }
    }



    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userid");
            var usernameClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "username");

            if (userIdClaim == null || usernameClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Token is invalid or claims are missing",
                    claims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value })
                });
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return Unauthorized(new { message = "User not found in database." });
            }

            var expClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "exp");

            if (expClaim == null || !long.TryParse(expClaim.Value, out var expUnix))
            {
                return Unauthorized(new { message = "Missing or invalid exp claim" });
            }

            var expirationDateUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

            var tokenBlackList = new TokenBlackList
            {
                UserId = user.Id, // string (Identity GUID)
                UserNumericId = user.UserId, //int (Vlastní ID)
                Token = token,
                ExpirationDate = expirationDateUtc
            };

            _dbContext.TokenBlackLists.Add(tokenBlackList);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "User logged out successfully.",
                userid = user.UserId,
                identityId = user.Id,
                username = user.UserName,
                claims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in Logout: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred.", exception = ex.Message });
        }
    }

}