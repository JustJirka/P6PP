namespace AdminSettings.Persistence.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
    public string Action { get; set; } = string.Empty;
    
}