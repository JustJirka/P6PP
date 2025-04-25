using System.ComponentModel.DataAnnotations;

namespace AuthService.API.DTO;

public class ChangePasswordModel
{
    [MinLength(6)] [Required] public string NewPassword { get; set; } = string.Empty;
    
    [Required]
    public string Token { get; set; } = string.Empty;
}