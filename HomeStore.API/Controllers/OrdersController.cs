using System.Security.Claims;
using HomeStore.Domain.DTOs.Orders;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiExplorerSettings(GroupName = "04. Orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) => _orderService = orderService;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var result = await _orderService.CreateOrderAsync(UserId, request);
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
        return result.Success ? Ok(result) : NotFound(result);
    }
}
