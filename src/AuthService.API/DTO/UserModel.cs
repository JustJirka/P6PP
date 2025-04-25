namespace AuthService.API.DTO;

public class UserModel
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? Sex { get; set; } = string.Empty;
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }

    // Navigation Property (optional)
    public Role? Role { get; set; }
}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class UserResponse
{
    public UserModel User { get; set; }
}