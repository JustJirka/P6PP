using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.API.Models
{
    public class TokenBlackList
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public required string UserId { get; set; }
        
        public int UserNumericId { get; set; } 
        
        [MaxLength(500)]
        public required string Token { get; set; }

        public DateTime ExpirationDate { get; set; }

        public ApplicationUser? User { get; set; }
    }
}