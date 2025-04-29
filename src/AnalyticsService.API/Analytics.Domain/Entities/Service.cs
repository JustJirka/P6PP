using System;
using System.Collections.Generic;

namespace Analytics.Domain.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public decimal Price { get; set; }
        public bool IsCancelled { get; set; }
        public int TrainerId { get; set; }
        public int RoomId { get; set; }
        
        //[ForeignKey(nameof(RoomId))]
        public Room Room { get; set; } = null!;
        public List<int> Users { get; set; } = new List<int>();
    }
}