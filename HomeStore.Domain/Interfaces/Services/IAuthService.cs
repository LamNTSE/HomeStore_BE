using HomeStore.Domain.DTOs.Auth;
using HomeStore.Domain.DTOs.Common;

namespace HomeStore.Domain.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>> GoogleLoginAsync(GoogleLoginRequest request);
    Task<ApiResponse<UserDto>> GetProfileAsync(int userId);
}
