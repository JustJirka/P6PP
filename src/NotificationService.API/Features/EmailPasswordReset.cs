using NotificationService.API.Persistence;
using NotificationService.API.Services;
using FluentValidation;
using ReservationSystem.Shared.Results;
namespace NotificationService.API.Features;

public record SendPasswordResetEmail(int Id, string url);
public record SendPasswordResetEmailResponse(int? Id=null);

public class SendPasswordResetEmailValidator : AbstractValidator<SendPasswordResetEmail>
{
    public SendPasswordResetEmailValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.url).NotEmpty();
    }
}

public class SendPasswordResetEmailHandler
{
    private readonly MailAppService _mailAppService;
    private readonly TemplateAppService _templateAppService;
    private readonly UserAppService _userAppService;

    public SendPasswordResetEmailHandler(MailAppService mailAppService,
        TemplateAppService templateAppService, UserAppService userAppService)
    {
        _mailAppService = mailAppService;
        _templateAppService = templateAppService;
        _userAppService = userAppService;
        
    }

    public async Task<ApiResult<SendPasswordResetEmailResponse>> Handle(SendPasswordResetEmail request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userAppService.GetUserByIdAsync(request.Id);
        
        if (user == null)
        {
            return new ApiResult<SendPasswordResetEmailResponse>(null, false, "User not found");
        }
        if (string.IsNullOrEmpty(user.Email) || (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))) 
        {
            return new ApiResult<SendPasswordResetEmailResponse>(null, false, "User email or name not found");
        }

        var template = await _templateAppService.GetTemplateAsync("PasswordReset");

        template.Text = template.Text.Replace("{name}", user.FirstName + " " +  user.LastName);
        template.Text = template.Text.Replace("{link}", request.url);

        var emailArgs = new EmailArgs
        {
            Address = new List<string> { user.Email },
            Subject = template.Subject,
            Body = template.Text
        };

        try
        {
            await _mailAppService.SendEmailAsync(emailArgs);
            return new ApiResult<SendPasswordResetEmailResponse>(new SendPasswordResetEmailResponse());
        }
        catch
        {
            return new ApiResult<SendPasswordResetEmailResponse>(null, false, "Email was not sent");
        }
    }
}

public static class SendPasswordResetEmailEndpoint
{
    public static void SendPasswordResetEmail(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/notification/user/sendpasswordresetemail",
            async (SendPasswordResetEmail request, SendPasswordResetEmailHandler handler, SendPasswordResetEmailValidator validator, CancellationToken cancellationToken) =>
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