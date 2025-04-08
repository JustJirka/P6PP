using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservationSystem.Shared.Clients;
using RoleService.API.Features;
using RoleService.API.Features.Roles;
using RoleService.API.Persistence;
using RoleService.API.Persistence.Repositories;
using RoleService.API.Services;

namespace RoleService.API.Extensions;

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
        services.AddScoped<RoleService>();
        services.AddScoped<Services.UserService>();
        
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

        // HttpClient
        services.AddHttpClient<NetworkHttpClient>();
        
        // Register Memory Cache
        services.AddMemoryCache();
    }
}