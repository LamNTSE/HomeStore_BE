using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Google.Apis.Auth;
using HomeStore.Domain.DTOs.Auth;
using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HomeStore.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepo, IMapper mapper, IConfiguration config)
    {
        _userRepo = userRepo;
        _mapper = mapper;
        _config = config;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepo.GetByEmailAsync(request.Email);
        if (existing != null)
            return ApiResponse<AuthResponse>.Fail("Email already exists.");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone = request.Phone,
            Address = request.Address,
            Role = "Customer",
            Provider = "Local"
        };

        await _userRepo.CreateAsync(user);
        var token = GenerateJwtToken(user);
        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserDto>(user)
        }, "Registration successful.");
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            return ApiResponse<AuthResponse>.Fail("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ApiResponse<AuthResponse>.Fail("Invalid email or password.");

        var token = GenerateJwtToken(user);
        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserDto>(user)
        });
    }

    public async Task<ApiResponse<AuthResponse>> GoogleLoginAsync(GoogleLoginRequest request)
    {
        try
        {
            var clientId = _config["Google:ClientId"];

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            var user = await _userRepo.GetByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new User
                {
                    FullName = payload.Name,
                    Email = payload.Email,
                    AvatarUrl = payload.Picture,
                    Provider = "Google",
                    Role = "Customer",
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepo.CreateAsync(user);
            }

            var token = GenerateJwtToken(user);

            return ApiResponse<AuthResponse>.Ok(new AuthResponse
            {
                Token = token,
                User = _mapper.Map<UserDto>(user)
            });
        }
        catch (InvalidJwtException ex)
        {
            return ApiResponse<AuthResponse>.Fail("Invalid Google token: " + ex.Message);
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponse>.Fail("Google authentication failed: " + ex.Message);
        }
    }

    public async Task<ApiResponse<UserDto>> GetProfileAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return ApiResponse<UserDto>.Fail("User not found.");
        return ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(user));
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
