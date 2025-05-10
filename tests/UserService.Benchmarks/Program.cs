using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using UserService.API.Abstraction;
using UserService.API.Persistence.Entities;
using UserService.API.Persistence.Repositories;
using UserService.API.Services;

namespace UserService.Benchmarks
{
    [MemoryDiagnoser]
    public class UserServiceBenchmarks
    {
        private IUserRepository _userRepo;
        private IRoleRepository _roleRepo;
        private IMemoryCache _memoryCache;
        private UserService.API.Services.UserService _userService;

        [GlobalSetup]
        public void Setup()
        {
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            // Shared data
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" },
                new Role { Id = 5, Name = "User" }
            };

            var users = new List<User>
            {
                new User { Id = 1, RoleId = 1 },
                new User { Id = 2, RoleId = 2 }
            };

            mockRoleRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(roles);
            mockUserRepo.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(users);
            mockUserRepo.Setup(r => r.GetTotalUserCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);

            mockUserRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(users[0]);
            mockRoleRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(roles[0]);

            mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(123);

            _userRepo = mockUserRepo.Object;
            _roleRepo = mockRoleRepo.Object;
            _userService = new UserService.API.Services.UserService(_userRepo, _memoryCache, _roleRepo);
        }

        [Benchmark]
        public async Task Benchmark_GetAllUsersAsync()
        {
            var result = await _userService.GetAllUsersAsync(1, 10, CancellationToken.None);
        }

        [Benchmark]
        public async Task Benchmark_GetUserByIdAsync()
        {
            var result = await _userService.GetUserByIdAsync(1, CancellationToken.None);
        }

        [Benchmark]
        public async Task Benchmark_AddUserAsync()
        {
            var user = new User { Email = "test@example.com" };
            var result = await _userService.AddUserAsync(user, CancellationToken.None);
        }
    }

    [MemoryDiagnoser]
    public class RoleServiceBenchmarks
    {
        private IRoleRepository _roleRepo;
        private IMemoryCache _memoryCache;
        private RoleService _roleService;

        private Role _sampleRole;

        [GlobalSetup]
        public void Setup()
        {
            var mockRoleRepo = new Mock<IRoleRepository>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            };

            _sampleRole = new Role { Id = 3, Name = "Tester" };

            mockRoleRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(roles);
            mockRoleRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(roles[0]);
            mockRoleRepo.Setup(r => r.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>())).ReturnsAsync(_sampleRole.Id);

            _roleRepo = mockRoleRepo.Object;
            _roleService = new RoleService(_roleRepo, _memoryCache);
        }

        [Benchmark]
        public async Task Benchmark_GetAllRolesAsync()
        {
            var result = await _roleService.GetAllRolesAsync(CancellationToken.None);
        }

        [Benchmark]
        public async Task Benchmark_GetRoleByIdAsync()
        {
            var result = await _roleService.GetRoleByIdAsync(1, CancellationToken.None);
        }

        [Benchmark]
        public async Task Benchmark_AddRoleAsync()
        {
            var result = await _roleService.AddRoleAsync(_sampleRole, CancellationToken.None);
        }
    }

    // Optional: entry point to run directly
    public class Program
    {
        public static void Main(string[] args)
        {
            // dotnet run -c Release
            BenchmarkRunner.Run<UserServiceBenchmarks>();
            BenchmarkRunner.Run<RoleServiceBenchmarks>();
        }
    }
}
