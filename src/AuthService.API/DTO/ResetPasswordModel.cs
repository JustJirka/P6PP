using System.ComponentModel.DataAnnotations;

namespace AuthService.API.DTO;

public class ResetPasswordModel
{
    public int UserId { get; set; }

    [MinLength(6)] [Required] public string NewPassword { get; set; }
}