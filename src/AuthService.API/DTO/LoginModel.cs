using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.API.DTO
{
    public class LoginModel
    {
        [Required]
        public string? UsernameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}