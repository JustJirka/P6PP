using NotificationService.API.Persistence;
using NotificationService.API.Services;
using FluentValidation;
using ReservationSystem.Shared.Results;
namespace NotificationService.API.Features;

public record SendVerificationEmail(int Id, string url);
public record SendVerificationEmailResponse(int? Id=null);

public class SendVerificationEmailValidator : AbstractValidator<SendVerificationEmail>
{
    public SendVerificationEmailValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.url).NotEmpty();
    }
}

public class SendVerificationEmailHandler
{
    private readonly MailAppService _mailAppService;
    private readonly TemplateAppService _templateAppService;
    private readonly UserAppService _userAppService;

    public SendVerificationEmailHandler(MailAppService mailAppService,
        TemplateAppService templateAppService, UserAppService userAppService)
    {
        _mailAppService = mailAppService;
        _templateAppService = templateAppService;
        _userAppService = userAppService;
        
    }

    public async Task<ApiResult<SendVerificationEmailResponse>> Handle(SendVerificationEmail request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userAppService.GetUserByIdAsync(request.Id);
        
        if (user == null)
        {
            return new ApiResult<SendVerificationEmailResponse>(null, false, "User not found");
        }
        if (string.IsNullOrEmpty(user.Email) || (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))) 
        {
            return new ApiResult<SendVerificationEmailResponse>(null, false, "User email or name not found");
        }

        var template = await _templateAppService.GetTemplateAsync("Verification");

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
            return new ApiResult<SendVerificationEmailResponse>(new SendVerificationEmailResponse());
        }
        catch
        {
            return new ApiResult<SendVerificationEmailResponse>(null, false, "Email was not sent");
        }
    }
}

public static class SendVerificationEmailEndpoint
{
    public static void SendVerificationEmail(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/notification/user/sendverificationemail",
            async (SendVerificationEmail request, SendVerificationEmailHandler handler, SendVerificationEmailValidator validator, CancellationToken cancellationToken) =>
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