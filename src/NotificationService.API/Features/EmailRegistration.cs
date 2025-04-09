using NotificationService.API.Persistence;
using NotificationService.API.Services;
using FluentValidation;
using ReservationSystem.Shared.Results;
namespace NotificationService.API.Features;

public record SendRegistrationEmail(int Id);
public record SendRegistrationEmailResponse(int? Id=null);

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

    public SendRegistrationEmailHandler(MailAppService mailAppService,
        TemplateAppService templateAppService, UserAppService userAppService)
    {
        _mailAppService = mailAppService;
        _templateAppService = templateAppService;
        _userAppService = userAppService;
        
    }

    public async Task<ApiResult<SendRegistrationEmailResponse>> Handle(SendRegistrationEmail request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userAppService.GetUserByIdAsync(request.Id);
        
        if (user == null)
        {
            return new ApiResult<SendRegistrationEmailResponse>(null, false, "User not found");
        }
        if (string.IsNullOrEmpty(user.Email) || (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))) 
        {
            return new ApiResult<SendRegistrationEmailResponse>(null, false, "User email or name not found");
        }
        
        var template = await _templateAppService.GetTemplateAsync("Registration");

        template.Text = template.Text.Replace("{name}", user.FirstName + " " +  user.LastName);

        var emailArgs = new EmailArgs
        {
            Address = new List<string> { user.Email },
            Subject = template.Subject,
            Body = template.Text
        };

        try
        {
            await _mailAppService.SendEmailAsync(emailArgs);
            return new ApiResult<SendRegistrationEmailResponse>(new SendRegistrationEmailResponse());
        }
        catch
        {
            return new ApiResult<SendRegistrationEmailResponse>(null, false, "Email was not sent");
        }
    }
}

public static class SendRegistrationEmailEndpoint
{
    public static void SendRegistrationEmail(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notification/user/sendregistrationemail/{id:int}",
            async (int id,SendRegistrationEmailHandler handler, SendRegistrationEmailValidator validator, CancellationToken cancellationToken) =>
            {
                var request = new SendRegistrationEmail(id);
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