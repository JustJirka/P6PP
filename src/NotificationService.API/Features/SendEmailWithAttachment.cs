using NotificationService.API.Services;
using FluentValidation;
using ReservationSystem.Shared;
using ReservationSystem.Shared.Results;
using ReservationSystem.Shared.Clients;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using NotificationService.API.Persistence;
using NotificationService.API.Persistence.Entities.DB;
using NotificationService.API.Logging; // <-- Přidáno pro FileLogger

namespace NotificationService.API.Features;

public record SendEmailWithAttachmentRequest(
    IList<string> Address,
    string Subject,
    string Body,
    IList<Base64Attachment> Attachments); // Použití Base64 pro přílohy

public record Base64Attachment(string FileName, string ContentBase64); // Nový model pro Base64 přílohy

public record SendEmailWithAttachmentResponse(int? Id = null);

public class SendEmailWithAttachmentRequestValidator : AbstractValidator<SendEmailWithAttachmentRequest>
{
    public SendEmailWithAttachmentRequestValidator()
    {
        RuleFor(x => x.Address).NotEmpty();
        RuleForEach(x => x.Address).EmailAddress();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(75);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(1500);
        RuleFor(x => x.Attachments).NotNull();
        RuleForEach(x => x.Attachments).ChildRules(attachments =>
        {
            attachments.RuleFor(x => x.FileName).NotEmpty();
            attachments.RuleFor(x => x.ContentBase64).NotEmpty();
        });
    }
}

public class SendEmailWithAttachmentHandler
{
    private readonly MailAppService _mailAppService;
    private readonly NetworkHttpClient _httpClient;

    public SendEmailWithAttachmentHandler(MailAppService mailAppService,
        NetworkHttpClient httpClient)
    {
        _httpClient = httpClient;
        _mailAppService = mailAppService;
    }

    public async Task<ApiResult<SendEmailWithAttachmentResponse>> Handle(SendEmailWithAttachmentRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var emailArgs = new EmailArgs
        {
            Address = request.Address,
            Subject = request.Subject,
            Body = request.Body
        };

        var attachments = new List<Attachment>();
        foreach (var base64Attachment in request.Attachments)
        {
            var contentBytes = Convert.FromBase64String(base64Attachment.ContentBase64);
            var stream = new MemoryStream(contentBytes);
            attachments.Add(new Attachment(stream, base64Attachment.FileName));
        }

        try
        {
            await _mailAppService.SendEmailAsync(emailArgs, attachments);

            // Logování úspěchu
            await FileLogger.LogInfo($"Email with attachments sent successfully to: {string.Join(", ", request.Address)} | Subject: {request.Subject}");

            return new ApiResult<SendEmailWithAttachmentResponse>(new SendEmailWithAttachmentResponse());
        }
        catch (Exception ex)
        {
            // Logování chyby
            await FileLogger.LogException(ex, $"Failed to send email with attachments to: {string.Join(", ", request.Address)} | Subject: {request.Subject}");

            return new ApiResult<SendEmailWithAttachmentResponse>(null, false, "Email with attachment was not sent");
        }
    }
}

public static class SendEmailWithAttachmentEndpoint
{
    public static void SendEmailWithAttachment(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/notification/sendemailwithattachment",
            async (SendEmailWithAttachmentRequest request, SendEmailWithAttachmentHandler handler, SendEmailWithAttachmentRequestValidator validator, CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    FileLogger.LogError($"Validation failed for SendEmailWithAttachmentRequest: {string.Join("; ", errorMessages)}"); // <-- Přidáno
                    return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.Handle(request, cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
    }
}
