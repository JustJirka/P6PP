using BookingService.API.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingService.API.Domain.Models;

[Table(nameof(Booking))]
public sealed class Booking : Entity<int>
{
    public DateTime BookingDate { get; set; }
    public BookingStatus Status { get; set; }

    public int UserId { get; set; }


    [ForeignKey(nameof(ServiceId))]
    public Service? Service { get; set; }
    public int ServiceId { get; set; }
}