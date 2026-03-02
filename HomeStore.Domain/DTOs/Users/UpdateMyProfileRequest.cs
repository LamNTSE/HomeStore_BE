namespace HomeStore.Domain.DTOs.Users;

public class UpdateMyProfileRequest
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    /// <summary>Optional: provide both OldPassword and NewPassword to change password.</summary>
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
}
