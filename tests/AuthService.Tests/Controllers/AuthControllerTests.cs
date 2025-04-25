using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using AuthService.API.Controllers;
using AuthService.API.Data;
using AuthService.API.DTO;
using AuthService.API.Interfaces;
using AuthService.API.Models;
using Castle.Core.Configuration;
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
    private readonly Mock<IUserAuthService> _mockAuthService;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        // Mock IUserAuthService
        _mockAuthService = new Mock<IUserAuthService>();

        // Create controller with mocked service
        _authController = new AuthController(_mockAuthService.Object);
    }

    /// <summary>
    /// Tests that Register returns Ok with a successful ApiResult for valid input.
    /// </summary>
    [Fact]
    public async Task Register_ValidUser_ReturnsOk()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        var model = new RegisterModel
        {
            Email = "test@example.com",
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "Test123!"
        };

        var expectedResult = new ApiResult<object>(new { Id = 1 }, true, "Success");

        // Setup mock to return success
        mockAuthService
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterModel>()))
            .ReturnsAsync(expectedResult);

        var controller = new AuthController(mockAuthService.Object);

        // Act
        var result = await controller.Register(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);
        Assert.True(apiResult.Success);
        Assert.Equal(1, ((dynamic)apiResult.Data).Id);
    }

    
    /// <summary>
    /// Tests that the Register method returns a BadRequest result when a user with the same username already exists.
    /// </summary>
    [Fact]
    public async Task Register_UserWithSameUsernameExists_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        var expectedResult = new ApiResult<object>(null, false, "User with this username already exists.");
        mockAuthService
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterModel>()))
            .ReturnsAsync(expectedResult);

        var authController = new AuthController(mockAuthService.Object);

        var model = new RegisterModel
        {
            Email = "test@example.com",
            UserName = "testuser", // Already existing username
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
        var mockAuthService = new Mock<IUserAuthService>();

        var expectedResult = new ApiResult<object>(null, false, "User with this email already exists.");
        mockAuthService
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterModel>()))
            .ReturnsAsync(expectedResult);

        var authController = new AuthController(mockAuthService.Object);

        var model = new RegisterModel
        {
            Email = "test@example.com", // Email already exists
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
        var mockAuthService = new Mock<IUserAuthService>();

        var expectedResult = new ApiResult<object>(null, false, "Error");
        mockAuthService
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterModel>()))
            .ReturnsAsync(expectedResult);

        var authController = new AuthController(mockAuthService.Object);

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
        Assert.False(apiResult.Success);
        Assert.Equal("Error", apiResult.Message);
    }

    /// <summary>
    /// Tests that the Register method returns a BadRequest result when the HTTP API call fails.
    /// </summary>
    [Fact]
    public async Task Register_HttpApiFails_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        // Simulate that RegisterAsync returns an ApiResult with a failure
        var expectedResult = new ApiResult<object>(null, false, "API Error");
        mockAuthService
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterModel>()))
            .ReturnsAsync(expectedResult);

        var authController = new AuthController(mockAuthService.Object);

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
        Assert.Equal("API Error", apiResult.Message);
    }

    /// <summary>
    /// Tests that the Register method returns a BadRequest result when the model is invalid.
    /// </summary>
    [Fact]
    public async Task Register_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        var model = new RegisterModel
        {
            Email = "",  // Invalid: Empty email
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "Test123!"
        };

        // Mock the RegisterAsync method to return a failure if model is invalid
        mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<RegisterModel>()))
            .ReturnsAsync(new ApiResult<object>(0, false, "Invalid model"));

        var authController = new AuthController(mockAuthService.Object);

        // Act
        var result = await authController.Register(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("Invalid model", apiResult.Message);
    }

    /// <summary>
    /// Tests that the Login method returns an Ok result with a token when the user credentials are valid.
    /// </summary>
    [Fact]
    public async Task Login_ValidUser_ReturnsOkWithToken()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();
        var expectedToken = "generated-jwt-token";

        mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginModel>()))
            .ReturnsAsync(new ApiResult<string>(expectedToken));

        var controller = new AuthController(mockAuthService.Object);
        var model = new LoginModel
        {
            UsernameOrEmail = "test@example.com",
            Password = "ValidPassword123!"
        };

        // Act
        var result = await controller.Login(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<string>>(okResult.Value);

        Assert.True(apiResult.Success);
        Assert.Equal(expectedToken, apiResult.Data);
        mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginModel>()), Times.Once);
    }

    /// <summary>
    /// Tests that the Login method returns a Bad Request result when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Login_InvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        // Mock the service to return failure response for non-existent user
        mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginModel>()))
            .ReturnsAsync(new ApiResult<string>(null, false, "Some error message"));

        var controller = new AuthController(mockAuthService.Object);

        var model = new LoginModel
        {
            UsernameOrEmail = "nonexistent@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await controller.Login(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<string>>(badRequestResult.Value);

        Assert.False(apiResult.Success);
        Assert.NotNull(apiResult.Message);

        // Verify the service was called with the correct model
        mockAuthService.Verify(x => x.LoginAsync(
            It.Is<LoginModel>(m =>
                m.UsernameOrEmail == "nonexistent@example.com" &&
                m.Password == "WrongPassword")),
            Times.Once);
    }

    /// <summary>
    /// Tests that the Login method returns a Bad Request result when the password is invalid.
    /// </summary>
    [Fact]
    public async Task Login_InvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        // Mock the service to return failure response for invalid password
        mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginModel>()))
            .ReturnsAsync(new ApiResult<string>(null, false, "Invalid credentials"));

        var controller = new AuthController(mockAuthService.Object);

        var model = new LoginModel
        {
            UsernameOrEmail = "test@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await controller.Login(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<string>>(badRequestResult.Value);

        Assert.False(apiResult.Success);
        Assert.NotNull(apiResult.Message);

        // Verify the service was called with correct parameters
        mockAuthService.Verify(x => x.LoginAsync(
            It.Is<LoginModel>(m =>
                m.UsernameOrEmail == "test@example.com" &&
                m.Password == "WrongPassword")),
            Times.Once);
    }

    /// <summary>
    /// Tests that the Login method returns a BadRequest result when the login data is invalid.
    /// </summary>
    [Fact]
    public async Task Login_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();
        var controller = new AuthController(mockAuthService.Object);

        var invalidModel = new LoginModel
        {
            UsernameOrEmail = "test@example.com",
            Password = "" // Empty password
        };

        controller.ModelState.AddModelError("Password", "The Password field is required.");

        // Act
        var result = await controller.Login(invalidModel);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
        Assert.False(apiResult.Success);
        Assert.Equal("Invalid data.", apiResult.Message);

        // Verify the service was NOT called (validation failed before reaching service)
        mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginModel>()), Times.Never);
    }

    /// <summary>
    /// Tests that the ResetPassword method returns Ok when the password is successfully reset.
    /// </summary>
    [Fact]
    public async Task ResetPassword_ValidRequest_ReturnsOk()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        // Mock successful password reset
        mockAuthService.Setup(x => x.ResetPasswordAsync(It.IsAny<ResetPasswordModel>()))
            .ReturnsAsync(new ApiResult<object>(
                new { UserId = 1, Email = "test@example.com" },
                true,
                "Password reset successfully."));

        var controller = new AuthController(mockAuthService.Object);

        var validModel = new ResetPasswordModel
        {
            NewPassword = "NewPassword123!",
            Token = "valid-token",
            userId = 1
        };

        // Act
        var result = await controller.ResetPassword(validModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);

        Assert.True(apiResult.Success);
        Assert.NotNull(apiResult.Message);

        // Verify the service was called with correct parameters
        mockAuthService.Verify(x => x.ResetPasswordAsync(
            It.Is<ResetPasswordModel>(m =>
                m.NewPassword == "NewPassword123!" &&
                m.Token == "valid-token" &&
                m.userId == 1)),
            Times.Once);
    }

    /// <summary>
    /// Tests that the ResetPassword method returns BadRequest when the model is invalid.
    /// </summary>
    [Fact]
    public async Task ResetPassword_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();
        var controller = new AuthController(mockAuthService.Object);

        var invalidModel = new ResetPasswordModel
        {
            NewPassword = "",
            Token = "some-token",
            userId = 1
        };

        controller.ModelState.AddModelError("NewPassword", "Password is required");

        // Act
        var result = await controller.ResetPassword(invalidModel);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);

        // Verify the service was NOT called
        mockAuthService.Verify(
            x => x.ResetPasswordAsync(It.IsAny<ResetPasswordModel>()),
            Times.Never);

    }

    /// <summary>
    /// Tests that the ResetPassword method returns BadRequest when the user is not found.
    /// </summary>
    [Fact]
    public async Task ResetPassword_UserNotFound_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();


        mockAuthService.Setup(x => x.ResetPasswordAsync(It.IsAny<ResetPasswordModel>()))
            .ReturnsAsync(new ApiResult<object>(null, false, "Some error message"));

        var controller = new AuthController(mockAuthService.Object);

        var model = new ResetPasswordModel
        {
            NewPassword = "NewPassword123!",
            Token = "valid-token",
            userId = 2 // Non-existent user ID
        };

        // Act
        var result = await controller.ResetPassword(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);

        Assert.False(apiResult.Success);
        Assert.NotNull(apiResult.Message);

        // Verify the service was called with correct parameters
        mockAuthService.Verify(x => x.ResetPasswordAsync(
            It.Is<ResetPasswordModel>(m =>
                m.NewPassword == "NewPassword123!" &&
                m.Token == "valid-token" &&
                m.userId == 2)),
            Times.Once);
    }

    /// <summary>
    /// Tests that the ResetPassword method returns BadRequest when authentication fails.
    /// </summary>
    [Fact]
    public async Task ResetPassword_AuthenticationFails_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        // Mock service to return failed response
        mockAuthService.Setup(x => x.ResetPasswordAsync(It.IsAny<ResetPasswordModel>()))
            .ReturnsAsync(new ApiResult<object>(null, false, "Some error message"));

        var controller = new AuthController(mockAuthService.Object);

        var model = new ResetPasswordModel
        {
            NewPassword = "NewPassword123!",
            Token = "invalid-token",
            userId = 1
        };

        // Act
        var result = await controller.ResetPassword(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);

        Assert.False(apiResult.Success);
        Assert.NotNull(apiResult.Message);

        // Verify the service was called
        mockAuthService.Verify(x => x.ResetPasswordAsync(
            It.Is<ResetPasswordModel>(m =>
                m.NewPassword == "NewPassword123!" &&
                m.Token == "invalid-token" &&
                m.userId == 1)),
            Times.Once);
    }

    /// <summary>
    /// Tests that the VerifyEmail method returns success when a valid token is provided.
    /// </summary>
    [Fact]
    public async Task VerifyEmail_ValidToken_ReturnsSuccess()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        // Mock successful email verification
        mockAuthService.Setup(x => x.VerifyEmailAsync(
            It.IsAny<int>(),
            It.IsAny<string>()))
            .ReturnsAsync(new ApiResult<object>(
                null,
                true,
                "Email verified successfully."));

        var controller = new AuthController(mockAuthService.Object);

        var userId = 4;
        var validToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("valid-token"));

        // Act
        var result = await controller.VerifyEmail(userId, validToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);

        Assert.True(apiResult.Success);
        Assert.NotNull(apiResult.Message);

        // Verify the service was called with correct parameters
        mockAuthService.Verify(x => x.VerifyEmailAsync(
            userId,
            validToken),
            Times.Once);
    }

    /// <summary>
    /// Tests that the VerifyEmail method returns BadRequest when an invalid token is provided.
    /// </summary>
    [Fact]
    public async Task VerifyEmail_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        mockAuthService.Setup(x => x.VerifyEmailAsync(
            It.IsAny<int>(),
            It.IsAny<string>()))
            .ReturnsAsync(new ApiResult<object>(
                null,
                false,
                "Some error message"));

        var controller = new AuthController(mockAuthService.Object);

        var userId = 5;
        var invalidToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("invalid-token"));

        // Act
        var result = await controller.VerifyEmail(userId, invalidToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);

        Assert.False(apiResult.Success);
        Assert.NotNull(apiResult.Message);

        // Verify the service was called with correct parameters
        mockAuthService.Verify(x => x.VerifyEmailAsync(
            userId,
            invalidToken),
            Times.Once);
    }

    /// <summary>
    /// Tests that the IsVerified method returns BadRequest when the user is not found.
    /// </summary>
    [Fact]
    public async Task IsVerified_UserNotFound_ReturnsBadRequest()
    {
        // Arrange

        var mockAuthService = new Mock<IUserAuthService>();

        mockAuthService.Setup(x => x.IsVerifiedAsync(It.IsAny<int>()))
            .ReturnsAsync(new ApiResult<object>(null, false, "Some error message"));

        var controller = new AuthController(mockAuthService.Object);
        var nonExistentUserId = 13;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        // Act
        var result = await controller.IsVerified(nonExistentUserId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);

        Assert.False(apiResult.Success);
        Assert.NotNull(apiResult.Message);

        // Verify the service was called with correct parameter
        mockAuthService.Verify(x => x.IsVerifiedAsync(nonExistentUserId), Times.Once);
    }

    /// <summary>
    /// Tests that the IsVerified method returns success when the user email is confirmed.
    /// </summary>
    [Fact]
    public async Task IsVerified_UserEmailConfirmed_ReturnsOk()
    {
        // Arrange
        var mockAuthService = new Mock<IUserAuthService>();

        mockAuthService.Setup(x => x.IsVerifiedAsync(It.IsAny<int>()))
            .ReturnsAsync(new ApiResult<object>(
                new { verified = true },
                true,
                "Some message"));

        var controller = new AuthController(mockAuthService.Object);
        var userId = 6;

        // Act
        var result = await controller.IsVerified(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<object>>(okResult.Value);

        Assert.True(apiResult.Success);
        Assert.NotNull(apiResult.Message);
        Assert.NotNull(apiResult.Data);

        // Verify the service was called with correct parameter
        mockAuthService.Verify(x => x.IsVerifiedAsync(userId), Times.Once);
    }

            /// <summary>
        /// Tests that the IsVerified method returns false when the user email is not confirmed.
        /// </summary>
        [Fact]
        public async Task IsVerified_UserEmailNotConfirmed_ReturnsUnverified()
        {
            // Arrange
            var mockAuthService = new Mock<IUserAuthService>();
    
            // Mock unverified email response
            mockAuthService.Setup(x => x.IsVerifiedAsync(It.IsAny<int>()))
                .ReturnsAsync(new ApiResult<object>(
                    new { verified = false }, 
                    false, 
                    "Some error message"));

            var controller = new AuthController(mockAuthService.Object);
            var userId = 7;

            // Act
            var result = await controller.IsVerified(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResult = Assert.IsType<ApiResult<object>>(badRequestResult.Value);
    
            Assert.False(apiResult.Success);
            Assert.NotNull(apiResult.Message);
            Assert.NotNull(apiResult.Data);
    
            // Verify the service was called with correct parameter
            mockAuthService.Verify(x => x.IsVerifiedAsync(userId), Times.Once);
        }

    /// <summary>
    /// Tests that the logout succeeds when a valid token is provided.
    /// </summary>
    [Fact]
    public async Task Logout_ValidToken_ReturnsOk()
    {
        // Arrange
        var token = "valid-token";
        var expectedData = new { message = "User logged out successfully." };
        var expectedResult = new ApiResult<object>(expectedData, true);

        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {token}";
        _authController.ControllerContext = new ControllerContext { HttpContext = context };

        _mockAuthService.Setup(s => s.LogoutAsync(context, token))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _authController.Logout();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedData, okResult.Value);
    }

    /// <summary>
    /// Tests that the logout fails when the token is invalid or missing required claims.
    /// </summary>
    [Fact]
    public async Task Logout_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var token = "invalid-token";
        var errorMessage = "Token is invalid or claims are missing";
        var expectedResult = new ApiResult<object>(
            new { message = errorMessage },
            false
        );

        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {token}";

        // Simulujate empty claims
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        context.User = claimsPrincipal;

        _authController.ControllerContext = new ControllerContext { HttpContext = context };

        _mockAuthService.Setup(s => s.LogoutAsync(context, token))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _authController.Logout();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(expectedResult.Data, unauthorizedResult.Value);
    }

    /// <summary>
    /// Tests that the logout fails when the user is not found in database.
    /// </summary>
    [Fact]
    public async Task Logout_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var token = "valid-token-but-user-not-found";
        var errorMessage = "User not found in database.";
        var expectedResult = new ApiResult<object>(
            new { message = errorMessage },
            false
        );

        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {token}";

        // Simulating claims with userid, but user is not in DB
        var claims = new[] {
        new Claim("userid", "123"),
        new Claim("username", "testuser")
    };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        context.User = claimsPrincipal;

        _authController.ControllerContext = new ControllerContext { HttpContext = context };

        _mockAuthService.Setup(s => s.LogoutAsync(context, token))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _authController.Logout();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(expectedResult.Data, unauthorizedResult.Value);
    }


}