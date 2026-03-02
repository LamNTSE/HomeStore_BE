namespace HomeStore.Domain.DTOs.Users;

public class AdminCreateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    /// <summary>Customer | Admin</summary>
    public string Role { get; set; } = "Customer";
    public string? AvatarUrl { get; set; }
}
