using System.ComponentModel.DataAnnotations;

namespace AuthService.API.DTO;

public class ResetPasswordModel
{
    [MinLength(6)] [Required] public string NewPassword { get; set; } = string.Empty;
}