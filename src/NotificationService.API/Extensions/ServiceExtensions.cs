using NotificationService.API.Services;
using NotificationService.API.Features;
using NotificationService.API.Persistence.Entities.DB;
using ReservationSystem.Shared.Clients;
using src.NotificationService.API.Services;

namespace NotificationService.API.Extensions;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MailAppService
        services.AddSingleton<MailAppService>();
        services.AddSingleton<UserAppService>();
        services.AddScoped<BookingAppService>();
        services.AddHostedService<TimerAppService>();

        // Register SendEmail Services
        services.AddSingleton<SendEmailRequestValidator>();
        services.AddScoped<SendEmailHandler>();

        // Register TemplateAppService
        services.AddSingleton<EditTemplateValidator>();
        services.AddScoped<TemplateAppService>();
        services.AddScoped<GetAllTemplatesHandler>();
        services.AddScoped<EditTemplateHandler>();

        // Register NotificationLogs
        services.AddSingleton<GetAllNotificationsValidator>();
        services.AddScoped<NotificationLogService>();
        services.AddScoped<GetAllNotificationsHandler>();
        services.AddScoped<SetNotificationsHandler>();

        // Register RegisterEmail Services
        services.AddSingleton<SendRegistrationEmailValidator>();
        services.AddScoped<SendRegistrationEmailHandler>();

        // Register VerificationEmail Services
        services.AddSingleton<SendVerificationEmailValidator>();
        services.AddScoped<SendVerificationEmailHandler>();

        // Register PasswordResetEmail Services
        services.AddSingleton<SendPasswordResetEmailValidator>();
        services.AddScoped<SendPasswordResetEmailHandler>();
        
        //Register BookingConfrimationEmail Services
        services.AddSingleton<SendBookingConfirmationEmailValidator>();
        services.AddScoped<SendBookingConfirmationEmailHandler>();

        // Register BookingCancellationEmail Services
        services.AddSingleton<SendBookingCancellationEmailValidator>();
        services.AddScoped<SendBookingCancellationEmailHandler>();
        
        // Register NetworkHttpClient
        services.AddHttpClient<NetworkHttpClient>();

        // Register SendEmailWithAttachment Services
        services.AddSingleton<SendEmailWithAttachmentRequestValidator>();
        services.AddScoped<SendEmailWithAttachmentHandler>();


    }
}

