namespace AdminSettings.Persistence.Entities;

public class Timezone
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string UtcOffset { get; set; }
}
