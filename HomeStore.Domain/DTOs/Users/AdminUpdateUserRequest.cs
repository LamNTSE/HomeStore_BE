namespace HomeStore.Domain.DTOs.Users;

public class AdminUpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    /// <summary>Customer | Admin</summary>
    public string? Role { get; set; }
    public string? NewPassword { get; set; }
}
