using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationService.API.Persistence.Entities.DB.Models;

public class Template : Entity
{
    [StringLength(100)]
    public required string Name { get; set; }
    [StringLength(75)]
    public required string Subject { get; set; }
    //Budou vytvoreny zastupne znaky pro vlozeni promennych(jmeno,datum,atd.)
    [StringLength(1500)]
    public required string Text { get; set; }
    [StringLength(10)]
    public string Language { get; set; } = "en";
}
