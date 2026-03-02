using System.Security.Claims;
using HomeStore.Domain.DTOs.Users;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "07. Users")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userService;

    public UsersController(IUserManagementService userService) => _userService = userService;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin => User.IsInRole("Admin");

    // ── Admin endpoints ──────────────────────────────────────────────────────

    /// <summary>Admin: get all users</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllUsersAsync();
        return Ok(result);
    }

    /// <summary>Admin: get user by id</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Admin: create a new user</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AdminCreate([FromBody] AdminCreateUserRequest request)
    {
        var result = await _userService.AdminCreateUserAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.UserId }, result) : BadRequest(result);
    }

    /// <summary>Admin: update any user</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> AdminUpdate(int id, [FromBody] AdminUpdateUserRequest request)
    {
        var result = await _userService.AdminUpdateUserAsync(id, request);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Admin: delete any user</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> AdminDelete(int id)
    {
        var result = await _userService.AdminDeleteUserAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    // ── Customer (own profile) ───────────────────────────────────────────────

    /// <summary>Customer: update own profile</summary>
    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileRequest request)
    {
        var result = await _userService.UpdateMyProfileAsync(UserId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Customer: delete own account</summary>
    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMyAccount()
    {
        var result = await _userService.DeleteMyAccountAsync(UserId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
