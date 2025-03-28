using System.ComponentModel.DataAnnotations.Schema;

namespace BookingPayments.API.Domain.Models;


[Table(nameof(Service))]
public sealed class Service : Entity<int>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Price { get; set; }
    public string? ServiceName { get; set; }
    public bool IsCancelled { get; set; }

    public int TrainerId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public Room? Room { get; set; }
    public int RoomId { get; set; }
}