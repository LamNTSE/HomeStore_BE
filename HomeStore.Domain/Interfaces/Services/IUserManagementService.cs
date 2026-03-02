using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Users;

namespace HomeStore.Domain.Interfaces.Services;

public interface IUserManagementService
{
    // Admin
    Task<ApiResponse<List<UserManagementDto>>> GetAllUsersAsync();
    Task<ApiResponse<UserManagementDto>> GetUserByIdAsync(int userId);
    Task<ApiResponse<UserManagementDto>> AdminCreateUserAsync(AdminCreateUserRequest request);
    Task<ApiResponse<UserManagementDto>> AdminUpdateUserAsync(int userId, AdminUpdateUserRequest request);
    Task<ApiResponse<bool>> AdminDeleteUserAsync(int userId);

    // Customer (own profile)
    Task<ApiResponse<UserManagementDto>> UpdateMyProfileAsync(int userId, UpdateMyProfileRequest request);
    Task<ApiResponse<bool>> DeleteMyAccountAsync(int userId);
}
