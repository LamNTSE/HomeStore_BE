using System.Security.Claims;
using HomeStore.Domain.DTOs.Payments;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "05. Payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService) => _paymentService = paymentService;

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _paymentService.CreatePaymentAsync(userId, request, HttpContext);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VnpayReturn()
    {
        var result = await _paymentService.HandleVnpayReturnAsync(Request.Query);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
