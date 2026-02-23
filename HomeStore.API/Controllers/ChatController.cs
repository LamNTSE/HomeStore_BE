using System.Security.Claims;
using HomeStore.Domain.DTOs.Chat;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiExplorerSettings(GroupName = "06. Chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService) => _chatService = chatService;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var result = await _chatService.SendMessageAsync(UserId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("conversation/{otherUserId}")]
    public async Task<IActionResult> GetConversation(int otherUserId)
    {
        var result = await _chatService.GetConversationAsync(UserId, otherUserId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyMessages()
    {
        var result = await _chatService.GetUserMessagesAsync(UserId);
        return Ok(result);
    }
}
