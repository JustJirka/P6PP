using System;
using Analytics.Domain.Enums;

namespace Analytics.Application.DTOs
{
    public class BookingDto
    {
        public int id { get; set; }
        //[JsonPropertyName("bookingDate")]
        public string bookingDate { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public int userId { get; set; }
        public int serviceId { get; set; }
        public ServiceDto? service { get; set; }
    }

    public class ServiceDto
    {
        public int trainerId { get; set; }
        public string serviceName { get; set; } = string.Empty;
        public string start { get; set; } = string.Empty; 
        public string end { get; set; } = string.Empty;
        public int roomId { get; set; }
        public RoomDto? room { get; set; }
        public List<int> users { get; set; } = new List<int>();
    }

    public class RoomDto
    {
        public string name { get; set; } = string.Empty;
        public int capacity { get; set; }
    }
}