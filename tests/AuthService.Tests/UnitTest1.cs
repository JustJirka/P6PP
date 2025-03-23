using System.Net;
using System.Net.Http.Json;
using AuthService.API.Controllers;
using AuthService.API.Data;
using AuthService.API.DTO;
using AuthService.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        // Mock UserManager behavior
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Mock HTTP Client
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
        var loggerMock = new Mock<ILogger<NetworkHttpClient>>();
        var networkHttpClient = new NetworkHttpClient(httpClient, loggerMock.Object);

        // Create AuthController with mocked dependencies
        var authController = new AuthController(_mockUserManager.Object, networkHttpClient, _configuration, _dbContext);

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
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);
        Assert.NotNull(result);
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
            UserId = 1, // Represents an existing user
            UserName = "testuser", // Same username as in the registration model
            Email = "existing@example.com" // Different email
        };

        // Mock UserManager to return the existing user when searching by username
        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Test registration data with the same username
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
            UserId = 1, // Represents an existing user
            UserName = "existinguser", // Different username
            Email = "test@example.com" // Same email as in the registration model
        };

        // Mock UserManager to return the existing user when searching by email
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Test registration data with the same email
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

        // Mock JWT configuration with a longer key
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:SecretKey"]).Returns("test-secret-key-12345678901234567890123456789012");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

        // Create AuthController with mocked dependencies
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
        Assert.NotNull(apiResult.Data); // Token should be in `Data`
        Assert.IsType<string>(apiResult.Data); // Expect the token to be a string
    }

    /// <summary>
    /// Tests that the Login method returns an Unauthorized result when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Login_InvalidUser_ReturnsUnauthorized()
    {
        // Arrange
        var userManagerMock = _mockUserManager;

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Test login data
        var model = new LoginModel
        {
            UsernameOrEmail = "nonexistent@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await authController.Login(model);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(unauthorizedResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("Invalid username/email or password.", apiResult.Message);
    }

    /// <summary>
    /// Tests that the Login method returns an Unauthorized result when the password is invalid.
    /// </summary>
    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
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

        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Test login data
        var model = new LoginModel
        {
            UsernameOrEmail = "test@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await authController.Login(model);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(unauthorizedResult.Value);
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
        Assert.Equal("Invalid data.", apiResult.Message);
    }

    /// <summary>
    /// Tests that the ResetPassword method returns an Ok result when the password is reset successfully.
    /// </summary>
    [Fact]
    public async Task ResetPassword_ValidPassword_ReturnsOk()
    {
        // Arrange
        var user = new ApplicationUser { UserId = 1, Email = "test@example.com" };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var model = new ResetPasswordModel { UserId = 1, NewPassword = "NewPassword123!" };
        var userManagerMock = _mockUserManager;

        userManagerMock
            .Setup(um => um.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("mockToken");

        userManagerMock
            .Setup(um => um.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var authController = new AuthController(userManagerMock.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Act
        var result = await authController.ResetPassword(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);
        Assert.True(apiResult.Success);
        Assert.Equal("Password reset successfully.", apiResult.Message);
    }

    /// <summary>
    /// Tests that the ResetPassword method returns a BadRequest result when the user does not exist.
    /// </summary>
    [Fact]
    public async Task ResetPassword_UserDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var model = new ResetPasswordModel { UserId = 999, NewPassword = "NewPassword123!" };

        var authController = new AuthController(_mockUserManager.Object, _mockHttpClient.Object, _configuration, _dbContext);

        // Act
        var result = await authController.ResetPassword(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("User with this ID does not exist.", apiResult.Message);
    }
}