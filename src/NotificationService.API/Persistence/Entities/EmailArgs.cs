namespace NotificationService.API.Persistence;

public class EmailArgs
{
    public required IList<string> Address { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
}