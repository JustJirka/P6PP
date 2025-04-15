using System.Text.Json.Serialization;
using Analytics.Domain.Enums;

namespace Analytics.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        [JsonPropertyName("bookingDate")]
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
    }
}