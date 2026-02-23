using System.Security.Claims;
using HomeStore.Domain.DTOs.Carts;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiExplorerSettings(GroupName = "03. Carts")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService) => _cartService = cartService;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var result = await _cartService.GetCartAsync(UserId);
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        var result = await _cartService.AddToCartAsync(UserId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemRequest request)
    {
        var result = await _cartService.UpdateCartItemAsync(UserId, cartItemId, request);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveCartItem(int cartItemId)
    {
        var result = await _cartService.RemoveCartItemAsync(UserId, cartItemId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var result = await _cartService.ClearCartAsync(UserId);
        return Ok(result);
    }
}
