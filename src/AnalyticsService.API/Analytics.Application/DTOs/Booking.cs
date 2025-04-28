using System.Text.Json.Serialization;

namespace Analytics.Application.DTOs
{
    public class BookingDto
    {
        public int id { get; set; }

        [JsonPropertyName("bookingDate")]
        public string bookingDate { get; set; }

        // Representing the enum as a string, e.g. "Confirmed", "Pending", "Cancelled"
        public string status { get; set; }

        public int userId { get; set; }
        public int serviceId { get; set; }
    }
}
