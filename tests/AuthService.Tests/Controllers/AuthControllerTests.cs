using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using AuthService.API.Controllers;
using AuthService.API.Data;
using AuthService.API.DTO;
using AuthService.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared.Results;

public class AuthControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly AuthDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly Mock<NetworkHttpClient> _mockHttpClient;

    public AuthControllerTests()
    {
        // Mock UserManager
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

        // Mock HttpClient
        _mockHttpClient = new Mock<NetworkHttpClient>(new HttpClient(), new Mock<ILogger<NetworkHttpClient>>().Object);

        // Use InMemory database for testing
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _dbContext = new AuthDbContext(options);

        // Mock IConfiguration
        var mockConfiguration = new Mock<IConfiguration>();
        _configuration = mockConfiguration.Object;

    }

    /// <summary>
    /// Tests that the Register method returns an Ok result when a valid user is provided for registration.
    /// </summary>
    [Fact]
    public async Task Register_ValidUser_ReturnsOk()
    {
        // Arrange
        var userManagerMock = _mockUserManager;

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("validToken");

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new ApiResult<object>(1, true, "Success"))
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        var networkHttpClient = new NetworkHttpClient(httpClient, new Mock<ILogger<NetworkHttpClient>>().Object);

        var authController = new AuthController(userManagerMock.Object, networkHttpClient, _configuration, _dbContext);

        var model = new RegisterModel
        {
            Email = "test@example.com",
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "Test123!"
        };

        // Act
        var result = await authController.Register(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);
        Assert.True(apiResult.Success);
    }

    /// <summary>
    /// Tests that the Register method returns a BadRequest result when a user with the same username already exists.
    /// </summary>
    [Fact]
    public async Task Register_UserWithSameUsernameExists_ReturnsBadRequest()
    {
        // Arrange
        var userManagerMock = _mockUserManager;

        // Simulate that a user with the same username already exists
        var existingUser = new ApplicationUser
        {
            UserId = 1,
            UserName = "testuser",
            Email = "existing@example.com"
        };

        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, _configuration, _dbContext);


        var model = new RegisterModel
        {
            Email = "test@example.com",
            UserName = "testuser", // Same username as existing user
            FirstName = "Test",
            LastName = "User",
            Password = "Test123!"
        };

        // Act
        var result = await authController.Register(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("User with this username already exists.", apiResult.Message);
    }

    /// <summary>
    /// Tests that the Register method returns a BadRequest result when a user with the same email already exists.
    /// </summary>
    [Fact]
    public async Task Register_UserWithSameEmailExists_ReturnsBadRequest()
    {
        // Arrange
        var userManagerMock = _mockUserManager;

        // Simulate that a user with the same email already exists
        var existingUser = new ApplicationUser
        {
            UserId = 1,
            UserName = "existinguser",
            Email = "test@example.com"
        };

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, _configuration, _dbContext);

        var model = new RegisterModel
        {
            Email = "test@example.com", // Same email as existing user
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "Test123!"
        };

        // Act
        var result = await authController.Register(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("User with this email already exists.", apiResult.Message);
    }

    /// <summary>
    /// Tests that the Register method returns a BadRequest result when user creation fails.
    /// </summary>
    [Fact]
    public async Task Register_CreateUserFails_ReturnsBadRequest()
    {
        // Arrange
        var userManagerMock = _mockUserManager;

        // Simulate that CreateAsync returns a failure
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Test registration data
        var model = new RegisterModel
        {
            Email = "test@example.com",
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "Test123!"
        };

        // Act
        var result = await authController.Register(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.NotNull(result);
        Assert.False(apiResult.Success);
    }

    /// <summary>
    /// Tests that the Register method returns a BadRequest result when the HTTP API call fails.
    /// </summary>
    [Fact]
    public async Task Register_HttpApiFails_ReturnsBadRequest()
    {
        // Arrange
        var userManagerMock = _mockUserManager;

        // Simulate that the API call returns an error
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError, // Simulate server error
                Content = JsonContent.Create(new ApiResult<object>(0, false, "API Error"))
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        var loggerMock = new Mock<ILogger<NetworkHttpClient>>();
        var networkHttpClient = new NetworkHttpClient(httpClient, loggerMock.Object);

        var authController = new AuthController(userManagerMock.Object, networkHttpClient, _configuration, _dbContext);

        // Test registration data
        var model = new RegisterModel
        {
            Email = "test@example.com",
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "Test123!"
        };

        // Act
        var result = await authController.Register(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.NotNull(result);
        Assert.False(apiResult.Success);
    }

    /// <summary>
    /// Tests that the Register method returns a BadRequest result when the model is invalid.
    /// </summary>
    [Fact]
    public async Task Register_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Invalid registration data (e.g., empty email)
        var model = new RegisterModel
        {
            Email = "",
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "Test123!"
        };

        // Act
        var result = await authController.Register(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(result);
    }

    /// <summary>
    /// Tests that the Login method returns an Ok result with a token when the user credentials are valid.
    /// </summary>
    [Fact]
    public async Task Login_ValidUser_ReturnsOkWithToken()
    {
        // Arrange
        var userManagerMock = _mockUserManager;

        var user = new ApplicationUser
        {
            UserId = 1,
            UserName = "testuser",
            Email = "test@example.com"
        };

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);

        // Mock JWT configuration
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["JWT_SECRET_KEY"]).Returns("test-secret-key-12345678901234567890123456789012");
        configMock.Setup(c => c["JWT_ISSUER"]).Returns("test-issuer");
        configMock.Setup(c => c["JWT_AUDIENCE"]).Returns("test-audience");

        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, configMock.Object, _dbContext);

        // Test login data
        var model = new LoginModel
        {
            UsernameOrEmail = "test@example.com",
            Password = "Test123!" // Valid password
        };

        // Act
        var result = await authController.Login(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<string>>(okResult.Value);
        Assert.True(apiResult.Success);
        Assert.NotNull(apiResult.Data);
        Assert.IsType<string>(apiResult.Data);
    }

    /// <summary>
    /// Tests that the Login method returns aBad Request result when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Login_InvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var userManagerMock = _mockUserManager;

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);


        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Test login data with a non-existent email/username and an incorrect password
        var model = new LoginModel
        {
            UsernameOrEmail = "nonexistent@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await authController.Login(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("Invalid username/email or password.", apiResult.Message);
    }

    /// <summary>
    /// Tests that the Login method returns a Bad Request result when the password is invalid.
    /// </summary>
    [Fact]
    public async Task Login_InvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserId = 1,
            UserName = "testuser",
            Email = "test@example.com"
        };

        var userManagerMock = _mockUserManager;

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
        userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false);

        // Create the AuthController with the mocked dependencies
        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Test login data with correct email/username but incorrect password
        var model = new LoginModel
        {
            UsernameOrEmail = "test@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await authController.Login(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("Invalid username/email or password.", apiResult.Message);
    }

    /// <summary>
    /// Tests that the Login method returns a BadRequest result when the login data is invalid.
    /// </summary>
    [Fact]
    public async Task Login_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Test login data
        var model = new LoginModel
        {
            UsernameOrEmail = "test@example.com",
            Password = "" // Empty password
        };

        // Act
        var result = await authController.Login(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("Invalid username/email or password.", apiResult.Message);
    }

    /// <summary>
    /// Tests that the ResetPassword method returns Ok when the password is successfully reset.
    /// </summary>
    [Fact]
    public async Task ResetPassword_ValidRequest_ReturnsOk()
    {
        // Arrange
        var userId = "1"; // Using string ID that matches IdentityUser.Id
        var user = new ApplicationUser
        {
            Id = userId,
            UserId = 1,
            UserName = "testuser",
            Email = "test@example.com"
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Setup UserManager mocks
        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync("valid-token");
        _mockUserManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Mock User claims
        var claims = new List<Claim> { new Claim("userid", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        authController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var model = new ResetPasswordModel { NewPassword = "NewPassword123!" };

        // Act
        var result = await authController.ResetPassword(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);
        Assert.True(apiResult.Success);
        Assert.Equal("Password reset successfully.", apiResult.Message);
    }

    /// <summary>
    /// Tests that the ResetPassword method returns Unauthorized when the user ID claim is missing.
    /// </summary>
    [Fact]
    public async Task ResetPassword_MissingUserIdClaim_ReturnsUnauthorized()
    {
        // Arrange
        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext);

        authController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var model = new ResetPasswordModel { NewPassword = "NewPassword123!" };

        // Act
        var result = await authController.ResetPassword(model);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(unauthorizedResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("Token does not contain user ID.", apiResult.Message);
    }

    /// <summary>
    /// Tests that the ResetPassword method returns BadRequest when the user is not found.
    /// </summary>
    [Fact]
    public async Task ResetPassword_UserNotFound_ReturnsBadRequest()
    {
        // Arrange
        var userId = "2";

        _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Mock User claims
        var claims = new List<Claim> { new Claim("userid", userId) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        authController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var model = new ResetPasswordModel { NewPassword = "NewPassword123!" };

        // Act
        var result = await authController.ResetPassword(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("User not found.", apiResult.Message);
    }



    /// <summary>
    /// Tests that the ResetPassword method returns BadRequest when the model is invalid.
    /// </summary>
    [Fact]
    public async Task ResetPassword_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Mock User claims
        var userId = "3";
        var claims = new List<Claim> { new Claim("userid", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        authController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Invalid model (empty password)
        var model = new ResetPasswordModel { NewPassword = "" };

        // Act
        var result = await authController.ResetPassword(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.NotNull(apiResult.Message);
    }

    /// <summary>
    /// Tests that the VerifyEmail method returns success when a valid token is provided.
    /// </summary>
    [Fact]
    public async Task VerifyEmail_ValidToken_ReturnsSuccess()
    {
        // Arrange
        var userId = "4";
        var user = new ApplicationUser
        {
            Id = userId,
            UserId = 4,
            UserName = "testuser",
            Email = "test@example.com",
            EmailConfirmed = false
        };

        // Add user to in-memory database
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext);

        var validToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("valid-token"));

        // Act
        var result = await authController.VerifyEmail(user.UserId, validToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);
        Assert.True(apiResult.Success);
        Assert.Equal("Email verified successfully.", apiResult.Message);

        // Verify the user was confirmed
        _mockUserManager.Verify(x => x.ConfirmEmailAsync(user, It.IsAny<string>()), Times.Once);
    }

    /// <summary>
    /// Tests that the VerifyEmail method returns failure when an invalid or expired token is provided.
    /// </summary>
    [Fact]
    public async Task VerifyEmail_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var userId = "5";
        var user = new ApplicationUser
        {
            Id = userId,
            UserId = 5,
            UserName = "testuser",
            Email = "test@example.com",
            EmailConfirmed = false
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid or expired token." }));

        var authController = new AuthController(
            _mockUserManager.Object,
            _mockHttpClient.Object,
            _configuration,
            _dbContext);

        var invalidToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("invalid-token"));

        // Act
        var result = await authController.VerifyEmail(user.UserId, invalidToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("Invalid or expired token.", apiResult.Message);

        // Verify that the email confirmation was attempted
        _mockUserManager.Verify(x => x.ConfirmEmailAsync(user, It.IsAny<string>()), Times.Once);
    }

    /// <summary>
    /// Tests that the IsVerified method returns BadRequest when the user is not found.
    /// </summary>
    [Fact]
    public async Task IsVerified_UserNotFound_ReturnsBadRequest()
    {
        // Arrange
        var userId = 13;

        var authController = new AuthController(
            _mockUserManager.Object,
            _mockHttpClient.Object,
            _configuration,
            _dbContext);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        // Act
        var result = await authController.IsVerified(userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("User not found.", apiResult.Message);

        Assert.Null(user);
    }

    /// <summary>
    /// Tests that the IsVerified method returns success when the user email is confirmed.
    /// </summary>
    [Fact]
    public async Task IsVerified_UserEmailConfirmed_ReturnsOk()
    {
        // Arrange
        var userId = "6";
        var user = new ApplicationUser
        {
            Id = userId,
            UserId = 6,
            UserName = "testuser",
            Email = "test@example.com",
            EmailConfirmed = true
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);

        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Act
        var result = await authController.IsVerified(user.UserId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);
        Assert.True(apiResult.Success);
        Assert.Equal("User is verified.", apiResult.Message);

        // Verify that the IsEmailConfirmedAsync was called once
        _mockUserManager.Verify(x => x.IsEmailConfirmedAsync(user), Times.Once);
    }

    /// <summary>
    /// Tests that the IsVerified method returns false when the user email is not confirmed.
    /// </summary>
    [Fact]
    public async Task IsVerified_UserEmailNotConfirmed_ReturnsOkWithFalse()
    {
        // Arrange
        var userId = "7";
        var user = new ApplicationUser
        {
            Id = userId,
            UserId = 7,
            UserName = "testuser",
            Email = "test@example.com",
            EmailConfirmed = false
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Act
        var result = await authController.IsVerified(user.UserId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("Email not verified.", apiResult.Message);

        // Verify that the IsEmailConfirmedAsync was called once
        _mockUserManager.Verify(x => x.IsEmailConfirmedAsync(user), Times.Once);
    }

    /// <summary>
    /// Tests that the Logout method successfully logs out the user and blacklists the token.
    /// </summary>
    [Fact]
    public async Task Logout_ValidToken_ReturnsOkAndBlacklistsToken()
    {
        // Arrange
        var userId = "8";
        var username = "testuser8";
        var token = "valid-token";
        var expirationDate = DateTime.UtcNow.AddMinutes(5);

        var user = new ApplicationUser
        {
            Id = userId,
            UserId = 8,
            UserName = username,
            Email = "test8@example.com"
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var claims = new List<Claim>
    {
        new Claim("userid", userId),
        new Claim("username", username),
        new Claim("exp", new DateTimeOffset(expirationDate).ToUnixTimeSeconds().ToString())
    };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.User).Returns(principal);
        mockHttpContext.Setup(x => x.Request.Headers["Authorization"]).Returns($"Bearer {token}");

        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };

        // Act
        var result = await authController.Logout();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

        // Verify token was added to blacklist
        var blacklistedToken = await _dbContext.TokenBlackLists.FirstOrDefaultAsync(t => t.Token == token);
        Assert.NotNull(blacklistedToken);
        Assert.Equal(userId, blacklistedToken.UserId);
        Assert.Equal(token, blacklistedToken.Token);
    }

    /// <summary>
    /// Tests that the Logout method returns Unauthorized when the user is not found in the database.
    /// </summary>
    [Fact]
    public async Task Logout_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var userId = "nonexistent-user";
        var username = "testuser";

        var claims = new List<Claim>
    {
        new Claim("userid", userId),
        new Claim("username", username)
    };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.User).Returns(principal);
        mockHttpContext.Setup(x => x.Request.Headers["Authorization"]).Returns("Bearer some-token");

        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };

        // Act
        var result = await authController.Logout();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    /// <summary>
    /// Tests that the Logout method returns Unauthorized when the exp claim is missing or invalid.
    /// </summary>
    [Fact]
    public async Task Logout_MissingExpClaim_ReturnsUnauthorized()
    {
        // Arrange
        var userId = "9";
        var username = "testuser9";

        var user = new ApplicationUser
        {
            Id = userId,
            UserId = 9,
            UserName = username,
            Email = "test@example.com"
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Missing exp claim
        var claims = new List<Claim>
    {
        new Claim("userid", userId),
        new Claim("username", username)
    };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.User).Returns(principal);
        mockHttpContext.Setup(x => x.Request.Headers["Authorization"]).Returns("Bearer some-token");

        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };

        // Act
        var result = await authController.Logout();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

}