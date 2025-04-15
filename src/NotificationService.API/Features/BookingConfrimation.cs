using NotificationService.API.Persistence;
using NotificationService.API.Services;
using FluentValidation;
using ReservationSystem.Shared.Results;
using NotificationService.API.Persistence.Entities;
using src.NotificationService.API.Services;
using src.NotificationService.API.Persistence.Entities.DB.Models;
using NotificationService.API.Logging; // <-- Přidáno

namespace NotificationService.API.Features;

public record SendBookingConfirmationEmailRequest(int UserId, int BookingId);
//TODO: Userid can be read from booking, if booking team add userId to API respond
public record SendBookingConfirmationEmailResponse(int? Id = null);

public class SendBookingConfirmationEmailValidator : AbstractValidator<SendBookingConfirmationEmailRequest>
{
    public SendBookingConfirmationEmailValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.BookingId).GreaterThan(0);
    }
}

public class SendBookingConfirmationEmailHandler
{
    private readonly MailAppService _mailAppService;
    private readonly TemplateAppService _templateAppService;
    private readonly UserAppService _userAppService;
    private readonly NotificationLogService _notificationLogService;
    private readonly BookingAppService _bookingAppService;

    public SendBookingConfirmationEmailHandler(MailAppService mailAppService,
                                               TemplateAppService templateAppService,
                                               UserAppService userAppService,
                                               NotificationLogService notificationLogService,
                                               BookingAppService bookingAppService)
    {
        _mailAppService = mailAppService;
        _templateAppService = templateAppService;
        _userAppService = userAppService;
        _notificationLogService = notificationLogService;
        _bookingAppService = bookingAppService;
    }

    public async Task<ApiResult<SendBookingConfirmationEmailResponse>> Handle(SendBookingConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userAppService.GetUserByIdAsync(request.UserId);
        if (user == null)
        {
            FileLogger.LogError($"User with ID {request.UserId} not found.");
            return new ApiResult<SendBookingConfirmationEmailResponse>(null, false, "User not found");
        }

        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
        {
            FileLogger.LogError($"User data incomplete (Email or Name) for ID {user.Id}");
            return new ApiResult<SendBookingConfirmationEmailResponse>(null, false, "User email or name not found");
        }

        ServiceResponse? service = null;
        try
        {
            service = await _bookingAppService.GetServiceByBookingIdAsync(request.BookingId);
        }
        catch (Exception ex)
        {
            await FileLogger.LogException(ex, $"Error fetching service for booking ID {request.BookingId}");
            return new ApiResult<SendBookingConfirmationEmailResponse>(null, false, "Event failed to load");
        }

        if (service == null)
        {
            FileLogger.LogError($"Service not found for booking ID {request.BookingId}");
            return new ApiResult<SendBookingConfirmationEmailResponse>(null, false, "Service not found in booking");
        }

        await _bookingAppService.SaveToTimer(request.BookingId, user.Id);

        var template = await _templateAppService.GetTemplateAsync("BookingConfirmation");

        template.Text = template.Text.Replace("{name}", $"{user.FirstName} {user.LastName}");
        template.Text = template.Text.Replace("{eventname}", service.serviceName);
        template.Text = template.Text.Replace("{datetime}", service.start.ToString());

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
                                                          NotificationType.ReservationConfirmed,
                                                          template.Subject,
                                                          template.Text);
            await FileLogger.LogInfo($"Confirmation email sent to user {user.Email} for booking ID {request.BookingId}");

            return new ApiResult<SendBookingConfirmationEmailResponse>(
                new SendBookingConfirmationEmailResponse(), true, "Confirmation email sent.");
        }
        catch (Exception ex)
        {
            await FileLogger.LogException(ex, $"Failed to send confirmation email to: {user.Email}");
            return new ApiResult<SendBookingConfirmationEmailResponse>(null, false, "Email was not sent");
        }
    }

    public static class SendBookingConfirmationEmailEndpoint
    {
        public static void SendBookingConfirmationEmail(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/notification/booking/sendbookingconfirmationemail",
                async (SendBookingConfirmationEmailRequest request, SendBookingConfirmationEmailHandler handler, SendBookingConfirmationEmailValidator validator, CancellationToken cancellationToken) =>
                {
                    var validationResult = await validator.ValidateAsync(request, cancellationToken);

                    if (!validationResult.IsValid)
                    {
                        var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                        await FileLogger.LogInfo("Validation failed for confirmation email request.");
                        return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                    }

                    var result = await handler.Handle(request, cancellationToken);

                    return result.Success
                        ? Results.Ok(result)
                        : Results.BadRequest(result);
                });
        }
    }
}
