using HomeStore.API.Hubs;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace HomeStore.API.Services;

public class CartNotificationService : ICartNotificationService
{
    private readonly IHubContext<CartHub> _cartHub;

    public CartNotificationService(IHubContext<CartHub> cartHub)
    {
        _cartHub = cartHub;
    }

    public async Task SendProductRemovedFromCartAsync(List<int> userIds, int productId)
    {
        foreach (var userId in userIds)
        {
            await _cartHub.Clients.Group($"cart_user_{userId}").SendAsync("ProductRemovedFromCart", new { ProductId = productId });
        }
    }
}
