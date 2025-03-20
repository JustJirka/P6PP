namespace ReservationSystem.Shared.Models.EmailTemplates;

public class WelcomeEmailModel
{
    public required string Name { get; set; }
    public required string ActivationLink { get; set; }
}