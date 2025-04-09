using BookingService.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingService.API.Domain.Models;

[Table(nameof(Room))]
public sealed class Room : Entity<int>
{
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    public int Capacity { get; set; }
    public RoomStatus Status { get; set; }

    public IList<Service> Services { get; set; } = [];
}