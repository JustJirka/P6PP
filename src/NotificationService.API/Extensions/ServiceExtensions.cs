using NotificationService.API.Services;
using NotificationService.API.Features;
using ReservationSystem.Shared.Clients;

namespace NotificationService.API.Extensions;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MailAppService
        services.AddSingleton<MailAppService>();
        services.AddSingleton<UserAppService>();

        // Register SendEmail Services
        services.AddSingleton<SendEmailRequestValidator>();
        services.AddScoped<SendEmailHandler>();

        // Register TemplateAppService
        services.AddScoped<TemplateAppService>();

        // Register RegisterEmail Services
        services.AddSingleton<SendRegistrationEmailValidator>();
        services.AddScoped<SendRegistrationEmailHandler>();

        // Register VerificationEmail Services
        services.AddSingleton<SendVerificationEmailValidator>();
        services.AddScoped<SendVerificationEmailHandler>();

        // Register PasswordResetEmail Services
        services.AddSingleton<SendPasswordResetEmailValidator>();
        services.AddScoped<SendPasswordResetEmailHandler>();

        // Register NetworkHttpClient
        services.AddHttpClient<NetworkHttpClient>();
    }
}

