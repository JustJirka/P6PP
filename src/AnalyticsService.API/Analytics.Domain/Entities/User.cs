using System.Text.Json.Serialization;
using Analytics.Domain.Enums;

namespace Analytics.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string State { get; set; } // e.g. "active", "inactive", "banned"
        public Sex? Sex { get; set; } = null;
        public int Weight { get; set; } // in grams
        public int Height { get; set; } // in mm

        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; set; }
    }
}
