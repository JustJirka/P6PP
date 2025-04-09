using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public required int UserId { get; set; }
        public ICollection<TokenBlackList> TokenBlackLists { get; set; } = new List<TokenBlackList>();
    }
}