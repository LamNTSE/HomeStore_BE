using System.Security.Claims;
using HomeStore.API.Hubs;
using HomeStore.Domain.DTOs.Chat;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiExplorerSettings(GroupName = "06. Chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHubContext<ChatHub> _chatHub;

    public ChatController(IChatService chatService, IHubContext<ChatHub> chatHub)
    {
        _chatService = chatService;
        _chatHub = chatHub;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var result = await _chatService.SendMessageAsync(UserId, request);
        if (result.Success && result.Data != null)
        {
            // Push message realtime to receiver
            await _chatHub.Clients
                .Group($"chat_user_{request.ReceiverId}")
                .SendAsync("ReceiveMessage", result.Data);
        }
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

    [HttpGet("partners")]
    public async Task<IActionResult> GetConversationPartners()
    {
        var result = await _chatService.GetConversationPartnersAsync(UserId);
        return Ok(result);
    }

    [HttpGet("admin")]
    public async Task<IActionResult> GetAdminUser()
    {
        var result = await _chatService.GetAdminUserAsync();
        return result.Success ? Ok(result) : NotFound(result);
    }
}
