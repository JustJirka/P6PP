using NotificationService.API.Persistence;
using NotificationService.API.Services;
using FluentValidation;
using ReservationSystem.Shared.Results;
using src.NotificationService.API.Persistence.Entities.DB.Models;
using src.NotificationService.API.Services;
using NotificationService.API.Logging; // <-- Přidáno

namespace NotificationService.API.Features;

public record SendRegistrationEmail(int Id);
public record SendRegistrationEmailResponse(int? Id = null);

public class SendRegistrationEmailValidator : AbstractValidator<SendRegistrationEmail>
{
    public SendRegistrationEmailValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class SendRegistrationEmailHandler
{
    private readonly MailAppService _mailAppService;
    private readonly TemplateAppService _templateAppService;
    private readonly UserAppService _userAppService;
    private readonly NotificationLogService _notificationLogService;

    public SendRegistrationEmailHandler(MailAppService mailAppService,
                                        TemplateAppService templateAppService,
                                        UserAppService userAppService,
                                        NotificationLogService notificationLogService)
    {
        _mailAppService = mailAppService;
        _templateAppService = templateAppService;
        _userAppService = userAppService;
        _notificationLogService = notificationLogService;
    }

    public async Task<ApiResult<SendRegistrationEmailResponse>> Handle(SendRegistrationEmail request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userAppService.GetUserByIdAsync(request.Id);

        if (user == null)
        {
            FileLogger.LogError($"User with ID {request.Id} not found.");
            return new ApiResult<SendRegistrationEmailResponse>(null, false, "User not found");
        }

        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
        {
             FileLogger.LogError($"User data incomplete (Email or Name) for ID {user.Id}");
            return new ApiResult<SendRegistrationEmailResponse>(null, false, "User email or name not found");
        }

        var template = await _templateAppService.GetTemplateAsync("Registration");

        template.Text = template.Text.Replace("{name}", $"{user.FirstName} {user.LastName}");

        var emailArgs = new EmailArgs
        {
            Address = new List<string> { user.Email },
            Subject = template.Subject,
            Body = template.Text
        };

        try
        {
            await _mailAppService.SendEmailAsync(emailArgs);

            await _notificationLogService.LogNotification(user.Id,
                                                          NotificationType.EmailRegistration,
                                                          template.Subject,
                                                          template.Text);

            await FileLogger.LogInfo($"Registration email successfully sent to user {user.Email}");

            return new ApiResult<SendRegistrationEmailResponse>(new SendRegistrationEmailResponse());
        }
        catch (Exception ex)
        {
            await FileLogger.LogException(ex, $"Failed to send registration email to: {user.Email}");
            return new ApiResult<SendRegistrationEmailResponse>(null, false, "Email was not sent");
        }
    }
}

public static class SendRegistrationEmailEndpoint
{
    public static void SendRegistrationEmail(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notification/user/sendregistrationemail/{id:int}",
            async (int id, SendRegistrationEmailHandler handler, SendRegistrationEmailValidator validator, CancellationToken cancellationToken) =>
            {
                var request = new SendRegistrationEmail(id);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    await FileLogger.LogInfo($"Validation failed for registration email request. ID: {id}");
                    return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.Handle(request, cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
    }
}
