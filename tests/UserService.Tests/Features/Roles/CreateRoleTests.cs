using FluentValidation;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using UserService.API.Features.Roles;
using UserService.API.Persistence.Entities;
using UserService.API.Persistence.Repositories;
using UserService.API.Services;
using UserService.API.Abstraction;
using Xunit;

namespace UserService.Tests.Features.Roles
{
    public class CreateRoleTests
    {
        private readonly CreateRoleHandler _handler;
        private readonly CreateRoleValidator _validator;

        public CreateRoleTests()
        {
            // Mock RoleRepository
            var mockRepo = new Mock<IRoleService>();

            // Mock AddAsync to return a fake ID
            mockRepo.Setup(r => r.AddRoleAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(42);

            // Mock MemoryCache
            var mockCache = new Mock<IMemoryCache>();
            object dummy;
            mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out dummy)).Returns(false);
            mockCache.Setup(c => c.CreateEntry(It.IsAny<object>()))
                     .Returns(Mock.Of<ICacheEntry>);

            // Real RoleService with mocked dependencies
            //var roleService = new RoleService(mockRepo.Object, mockCache.Object);
            _handler = new CreateRoleHandler(mockRepo.Object);
            _validator = new CreateRoleValidator();
        }

        /// <summary>
        /// Tests that creating a valid role returns a successful result
        /// with the expected role ID and no validation errors.
        /// </summary>
        [Fact]
        public async Task CreateRole_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var request = new CreateRoleRequest("Admin", "Has power");

            // Act
            var validation = await _validator.ValidateAsync(request);
            var result = await _handler.HandleAsync(request, CancellationToken.None);

            // Assert
            Assert.True(validation.IsValid);
            Assert.True(result.Success);
            Assert.Equal(42, result.Data);
        }

        /// <summary>
        /// Tests that submitting an invalid role creation request
        /// (with empty name and description) returns validation errors.
        /// </summary>
        [Fact]
        public async Task CreateRole_InvalidRequest_ReturnsValidationErrors()
        {
            var request = new CreateRoleRequest("", "");

            var validation = await _validator.ValidateAsync(request);

            Assert.False(validation.IsValid);
            Assert.Contains(validation.Errors, e => e.PropertyName == "Name");
            Assert.Contains(validation.Errors, e => e.PropertyName == "Description");
        }

        /// <summary>
        /// Tests that if the role repository fails to add the role (returns null),
        /// the handler returns a failed result with the correct error message.
        /// </summary>
        [Fact]
        public async Task CreateRole_AddRoleReturnsNull_ReturnsFailure()
        {
            // Arrange: Setup IRoleService to return null
            var mockRepo = new Mock<IRoleService>();
            mockRepo.Setup(r => r.AddRoleAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((int?)null);

            var handler = new CreateRoleHandler(mockRepo.Object);
            var request = new CreateRoleRequest("Fail", "Should fail");

            // Act
            var result = await handler.HandleAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Role not created", result.Message);
        }
    }
}
