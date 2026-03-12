using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HomeStore.API.Hubs;

[Authorize]
public class OrderHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = Context.User?.FindFirstValue(ClaimTypes.Role);

        if (userId != null)
        {
            if ("Admin".Equals(role, StringComparison.OrdinalIgnoreCase))
                await Groups.AddToGroupAsync(Context.ConnectionId, "admin_orders");
            else
                await Groups.AddToGroupAsync(Context.ConnectionId, $"order_user_{userId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = Context.User?.FindFirstValue(ClaimTypes.Role);

        if (userId != null)
        {
            if ("Admin".Equals(role, StringComparison.OrdinalIgnoreCase))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "admin_orders");
            else
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order_user_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
