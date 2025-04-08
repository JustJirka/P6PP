using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservationSystem.Shared.Clients;
using PaymentService.API.Features;
using PaymentService.API.Features.Payments;
using PaymentService.API.Persistence;
using PaymentService.API.Persistence.Repositories;
using PaymentService.API.Services;
using ExampleService.API.Persistence;
using DatabaseInitializer = PaymentService.API.Persistence.DatabaseInitializer;

namespace PaymentService.API.Extensions;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Db Context 
        services.AddSingleton<DapperContext>();
        services.AddSingleton<DatabaseInitializer>();
        
        // Register Repositories
        services.AddScoped<PaymentRepository>();
        
        // Register Services
        services.AddScoped<Services.PaymentService>();
       
        
        // Register Endpoints injections
        services.AddScoped<CreateBalanceHandler>();
        services.AddSingleton<CreateBalanceValidator>();

        services.AddScoped<UpdatePaymentHandler>();
        services.AddSingleton<UpdatePaymentValidator>();

        services.AddScoped<GetBalanceByIdHandler>();
        services.AddSingleton<GetBalanceByIdValidator>();

        services.AddScoped<GetPaymentByIdHandler>();
        services.AddSingleton<GetPaymentByIdValidator>();

        services.AddScoped<CreatePaymentHandler>();
        services.AddSingleton<CreatePaymentValidator>();

        // HttpClient
        services.AddHttpClient<NetworkHttpClient>();
        
        // Register Memory Cache
        services.AddMemoryCache();
    }
}