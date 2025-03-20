namespace NotificationService.API.Services;

using ReservationSystem.Shared.Models.EmailTemplates;
using ReservationSystem.Shared.Services;

public class EmailServiceExample
{
    private readonly EmailTemplateService _templateService;
    
    public EmailServiceExample(EmailTemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task SendWelcomeEmail(string email, string name, string activationLink)
    {
        var model = new WelcomeEmailModel
        {
            Name = name,
            ActivationLink = activationLink
        };

        var emailBody = await _templateService.RenderTemplateAsync("WelcomeEmail", model);

        Console.WriteLine($"Sending email to {email}:\n{emailBody}");
    }
}
