using System.ComponentModel.DataAnnotations;

namespace AuthService.API.DTO;

public class ResetPasswordModel
{
    [MinLength(6)] [Required] public string NewPassword { get; set; } = string.Empty;
    
    [Required]
    public string Token { get; set; } = string.Empty;
    
    public int userId { get; set; } 
}