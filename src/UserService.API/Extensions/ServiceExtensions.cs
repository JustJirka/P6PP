using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservationSystem.Shared.Clients;
using UserService.API.Abstraction;
using UserService.API.Features;
using UserService.API.Features.Roles;
using UserService.API.Persistence;
using UserService.API.Persistence.Repositories;
using UserService.API.Services;

namespace UserService.API.Extensions;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Db Context 
        services.AddSingleton<DapperContext>();
        services.AddSingleton<DatabaseInitializer>();
        services.AddSingleton<DatabaseSeeder>();
        
        // Register Repositories
        services.AddScoped<RoleRepository>();
        services.AddScoped<UserRepository>();
        
        // Register Services
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, Services.UserService>();
        
        // Register Endpoints injections
        services.AddScoped<GetUserByIdHandler>();
        services.AddSingleton<GetUserByIdValidator>();
        
        services.AddScoped<DeleteUserHandler>();
        services.AddSingleton<DeleteUserValidator>();

        services.AddScoped<UpdateUserHandler>();
        services.AddSingleton<UpdateUserValidator>();

        services.AddScoped<CreateUserHandler>();
        services.AddSingleton<CreateUserValidator>();
        
        services.AddScoped<GetUsersHandler>();

        services.AddScoped<GetRolesHandler>();
        
        services.AddScoped<GetRoleByIdHandler>();
        services.AddSingleton<GetRoleByIdValidator>();
        
        services.AddScoped<CreateRoleHandler>();
        services.AddSingleton<CreateRoleValidator>();

        services.AddScoped<AssignUserRoleHandler>();
        services.AddSingleton<AssignUserRoleValidator>();
        
        services.AddScoped<DeactivateUserHandler>();
        services.AddSingleton<DeactivateUserValidator>();
        
        services.AddScoped<ActivateUserHandler>();
        services.AddSingleton<ActivateUserValidator>();

        // HttpClient
        services.AddHttpClient<NetworkHttpClient>();
        
        // Register Memory Cache
        services.AddMemoryCache();
    }
}