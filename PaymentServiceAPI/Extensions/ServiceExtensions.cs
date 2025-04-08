using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservationSystem.Shared.Clients;
using PaymentService.API.Features;
using PaymentService.API.Features.Roles;
using PaymentService.API.Persistence;
using PaymentService.API.Persistence.Repositories;
using PaymentService.API.Services;

namespace PaymentService.API.Extensions;

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
        services.AddScoped<Services.PaymentService>();
        services.AddScoped<Services.CreditService>();
        
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
        
        services.AddScoped<CreatePaymentHandler>();
        services.AddSingleton<CreatePaymentValidator>();

        services.AddScoped<AssignUserRoleHandler>();
        services.AddSingleton<AssignUserRoleValidator>();

        // HttpClient
        services.AddHttpClient<NetworkHttpClient>();
        
        // Register Memory Cache
        services.AddMemoryCache();
    }
}