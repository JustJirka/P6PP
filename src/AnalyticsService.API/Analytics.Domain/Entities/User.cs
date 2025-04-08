using Analytics.Domain.Enums;

namespace Analytics.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string State { get; set; } // e.g. "active", "inactive", "banned"
        public Sex Sex { get; set; }
        public int Weight { get; set; } // in grams
        public int Height { get; set; } // in mm
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
