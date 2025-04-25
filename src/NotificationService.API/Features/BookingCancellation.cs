using NotificationService.API.Persistence;
using NotificationService.API.Services;
using FluentValidation;
using NotificationService.API.Persistence.Entities;
using ReservationSystem.Shared.Results;
using src.NotificationService.API.Persistence.Entities.DB.Models;
using src.NotificationService.API.Services;
using NotificationService.API.Logging; // <-- Přidáno

namespace NotificationService.API.Features;

public record SendBookingCancellationEmailRequest(int BookingId);
public record SendBookingCancellationEmailResponse(int? Id = null);

public class SendBookingCancellationEmailValidator : AbstractValidator<SendBookingCancellationEmailRequest>
{
    public SendBookingCancellationEmailValidator()
    {
        RuleFor(x => x.BookingId).GreaterThan(0);
    }
}

public class SendBookingCancellationEmailHandler
{
    private readonly MailAppService _mailAppService;
    private readonly TemplateAppService _templateAppService;
    private readonly UserAppService _userAppService;
    private readonly NotificationLogService _notificationLogService;
    private readonly BookingAppService _bookingAppService;

    public SendBookingCancellationEmailHandler(MailAppService mailAppService,
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

    public async Task<ApiResult<SendBookingCancellationEmailResponse>> Handle(SendBookingCancellationEmailRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        BookingResponse? booking = null;
        try
        {
            booking = await _bookingAppService.GetBookingByIdAsync(request.BookingId);

        }
        catch (Exception e)
        {
            FileLogger.LogError($"Booking with ID {request.BookingId} throw exception: {e.Message}");

        }

        var user = await _userAppService.GetUserByIdAsync(booking.userId);

        if (user == null)
        {
            FileLogger.LogError($"User with ID {booking.userId} not found.");
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "User not found");
        }

        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
        {
            FileLogger.LogError($"User data incomplete (Email or Name) for ID {user.Id}");
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "User email or name not found");
        }

        ServiceResponse? service = null;
        try
        {
            service = await _bookingAppService.GetServiceByIdAsync(booking.serviceId);
        }
        catch (Exception ex)
        {
            await FileLogger.LogException(ex, $"Error fetching service for booking ID {request.BookingId}");
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "Event failed to load");
        }

        if (service == null)
        {
            FileLogger.LogError($"Service not found for booking ID {request.BookingId}");
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "Event not found");
        }

        await _bookingAppService.DeleteTimer(request.BookingId);

        var template = await _templateAppService.GetTemplateAsync("BookingCancellation");

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
                                                          NotificationType.ReservationCanceled,
                                                          template.Subject,
                                                          template.Text);
            await FileLogger.LogInfo($"Cancellation email sent to user {user.Email} for booking ID {request.BookingId}");

            return new ApiResult<SendBookingCancellationEmailResponse>(
                new SendBookingCancellationEmailResponse(), true, "Cancellation email sent.");
        }
        catch (Exception ex)
        {
            await FileLogger.LogException(ex, $"Failed to send cancellation email to: {user.Email}");
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "Email was not sent");
        }
    }

    public static class SendBookingCancellationEmailEndpoint
    {
        public static void SendBookingCancellationEmail(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/notification/booking/sendbookingcancellationemail",
                async (SendBookingCancellationEmailRequest request, SendBookingCancellationEmailHandler handler, SendBookingCancellationEmailValidator validator, CancellationToken cancellationToken) =>
                {
                    var validationResult = await validator.ValidateAsync(request, cancellationToken);

                    if (!validationResult.IsValid)
                    {
                        var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                        await FileLogger.LogInfo("Validation failed for cancellation email request.");
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
