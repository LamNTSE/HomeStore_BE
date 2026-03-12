using System.Security.Claims;
using HomeStore.API.Hubs;
using HomeStore.Domain.DTOs.Orders;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiExplorerSettings(GroupName = "04. Orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IHubContext<OrderHub> _orderHub;

    public OrdersController(IOrderService orderService, IHubContext<OrderHub> orderHub)
    {
        _orderService = orderService;
        _orderHub = orderHub;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var result = await _orderService.CreateOrderAsync(UserId, request);
        if (result.Success && result.Data != null)
            await _orderHub.Clients.Group("admin_orders").SendAsync("NewOrderCreated", result.Data);
        return result.Success ? CreatedAtAction(nameof(GetOrder), new { id = result.Data!.OrderId }, result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var result = await _orderService.GetOrderByIdAsync(UserId, id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var result = await _orderService.GetUserOrdersAsync(UserId);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _orderService.GetAllOrdersAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, status);
        if (result.Success && result.Data != null)
        {
            // Notify the customer who owns the order
            await _orderHub.Clients
                .Group($"order_user_{result.Data.UserId}")
                .SendAsync("OrderStatusUpdated", result.Data);
            // Notify admin panel to stay in sync
            await _orderHub.Clients.Group("admin_orders").SendAsync("OrderStatusUpdated", result.Data);
        }
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var result = await _orderService.CancelOrderAsync(UserId, id);
        if (result.Success && result.Data != null)
            await _orderHub.Clients.Group("admin_orders").SendAsync("OrderStatusUpdated", result.Data);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}/confirm-delivery")]
    public async Task<IActionResult> ConfirmDelivery(int id)
    {
        var result = await _orderService.ConfirmDeliveryAsync(UserId, id);
        if (result.Success && result.Data != null)
            await _orderHub.Clients.Group("admin_orders").SendAsync("OrderStatusUpdated", result.Data);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
