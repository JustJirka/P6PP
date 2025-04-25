namespace NotificationService.API.Persistence.Entities.DB.Models;

public class Booking : Entity
{
    public required int BookingId { get; set; }
    public required DateTime Start{ get; set; }
    public bool Notice24H { get; set; } = false;
}