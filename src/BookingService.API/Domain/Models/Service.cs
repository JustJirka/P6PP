using System.ComponentModel.DataAnnotations.Schema;

namespace BookingService.API.Domain.Models;


[Table(nameof(Service))]
public sealed class Service : Entity<int>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Price { get; set; }
    public string? ServiceName { get; set; }
    public bool IsCancelled { get; set; }
    //public string ServiceDescription { get; set; }
    public int TrainerId { get; set; }
    public List<int> Users { get; set; } = [];
    [ForeignKey(nameof(RoomId))]
    public int RoomId { get; set; }
    public Room? Room { get; set; }
}