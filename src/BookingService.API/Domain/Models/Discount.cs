using System.ComponentModel.DataAnnotations.Schema;

namespace BookingService.API.Domain.Models;

[Table(nameof(Discount))]
public sealed class Discount : Entity<int>
{
    public int Percentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsValid { get; set; }
}