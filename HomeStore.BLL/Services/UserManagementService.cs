using AutoMapper;
using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Users;
using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;

namespace HomeStore.BLL.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;

    public UserManagementService(IUserRepository userRepo, IMapper mapper)
    {
        _userRepo = userRepo;
        _mapper = mapper;
    }

    // ── Admin ────────────────────────────────────────────────────────────────

    public async Task<ApiResponse<List<UserManagementDto>>> GetAllUsersAsync()
    {
        var users = await _userRepo.GetAllAsync();
        return ApiResponse<List<UserManagementDto>>.Ok(_mapper.Map<List<UserManagementDto>>(users));
    }

    public async Task<ApiResponse<UserManagementDto>> GetUserByIdAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return ApiResponse<UserManagementDto>.Fail("User not found.");
        return ApiResponse<UserManagementDto>.Ok(_mapper.Map<UserManagementDto>(user));
    }

    public async Task<ApiResponse<UserManagementDto>> AdminCreateUserAsync(AdminCreateUserRequest request)
    {
        var existing = await _userRepo.GetByEmailAsync(request.Email);
        if (existing != null) return ApiResponse<UserManagementDto>.Fail("Email already exists.");

        var role = request.Role == "Admin" ? "Admin" : "Customer";
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone = request.Phone,
            Address = request.Address,
            Role = role,
            AvatarUrl = request.AvatarUrl,
            Provider = "Local"
        };

        await _userRepo.CreateAsync(user);
        return ApiResponse<UserManagementDto>.Ok(_mapper.Map<UserManagementDto>(user), "User created.");
    }

    public async Task<ApiResponse<UserManagementDto>> AdminUpdateUserAsync(int userId, AdminUpdateUserRequest request)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return ApiResponse<UserManagementDto>.Fail("User not found.");

        if (request.FullName != null) user.FullName = request.FullName;
        if (request.Phone != null) user.Phone = request.Phone;
        if (request.Address != null) user.Address = request.Address;
        if (request.AvatarUrl != null) user.AvatarUrl = request.AvatarUrl;
        if (request.Role != null && (request.Role == "Admin" || request.Role == "Customer"))
            user.Role = request.Role;
        if (!string.IsNullOrWhiteSpace(request.NewPassword))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _userRepo.UpdateAsync(user);
        return ApiResponse<UserManagementDto>.Ok(_mapper.Map<UserManagementDto>(user), "User updated.");
    }

    public async Task<ApiResponse<bool>> AdminDeleteUserAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return ApiResponse<bool>.Fail("User not found.");
        await _userRepo.DeleteAsync(userId);
        return ApiResponse<bool>.Ok(true, "User deleted.");
    }

    // ── Customer (own profile) ───────────────────────────────────────────────

    public async Task<ApiResponse<UserManagementDto>> UpdateMyProfileAsync(int userId, UpdateMyProfileRequest request)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return ApiResponse<UserManagementDto>.Fail("User not found.");

        if (request.FullName != null) user.FullName = request.FullName;
        if (request.Phone != null) user.Phone = request.Phone;
        if (request.Address != null) user.Address = request.Address;
        if (request.AvatarUrl != null) user.AvatarUrl = request.AvatarUrl;

        if (!string.IsNullOrWhiteSpace(request.OldPassword) && !string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                return ApiResponse<UserManagementDto>.Fail("Old password is incorrect.");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        }

        await _userRepo.UpdateAsync(user);
        return ApiResponse<UserManagementDto>.Ok(_mapper.Map<UserManagementDto>(user), "Profile updated.");
    }

    public async Task<ApiResponse<bool>> DeleteMyAccountAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return ApiResponse<bool>.Fail("User not found.");
        await _userRepo.DeleteAsync(userId);
        return ApiResponse<bool>.Ok(true, "Account deleted.");
    }
}
