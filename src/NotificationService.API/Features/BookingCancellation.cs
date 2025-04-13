using NotificationService.API.Persistence;
using NotificationService.API.Services;
using FluentValidation;
using NotificationService.API.Persistence.Entities;
using ReservationSystem.Shared.Results;
using src.NotificationService.API.Persistence.Entities.DB.Models;
using src.NotificationService.API.Services;
namespace NotificationService.API.Features;

public record SendBookingCancellationEmailRequest(int UserId, int BookingId);
//TODO: Userid can be read from booking, if booking team add userId to API respond
public record SendBookingCancellationEmailResponse(int? Id = null);

public class SendBookingCancellationEmailValidator : AbstractValidator<SendBookingCancellationEmailRequest>
{
    public SendBookingCancellationEmailValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
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
                                               BookingAppService bookingAppService
    )
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
        var user = await _userAppService.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "User not found");
        }
        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
        {
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "User email or name not found");
        }
        
        ServiceResponse? service = null;
        try
        {
            service = await _bookingAppService.GetServiceByBookingIdAsync(request.UserId);
            
        }
        catch (Exception ex)
        {
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "Event failed to load");
        }
      
        if (service == null)
        {
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "Event not found");
        }
        await _bookingAppService.DeleteTimer(request.BookingId);
        var template = await _templateAppService.GetTemplateAsync("BookingCancellation");

        template.Text = template.Text.Replace("{name}", user.FirstName + " " + user.LastName);
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
                                                          template.Text
            );
            return new ApiResult<SendBookingCancellationEmailResponse>(new SendBookingCancellationEmailResponse());
        }
        catch
        {
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "Email was not sent");
        }
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
                    return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.Handle(request, cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
    }
}
