using HomeStore.Domain.DTOs.Payments;
using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "05. Payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // ===============================
    // CREATE PAYMENT
    // ===============================

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _paymentService.CreatePaymentAsync(
            userId,
            request,
            HttpContext);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    // ===============================
    // VNPAY RETURN
    // ===============================

    [HttpGet("vnpay-return")]
    [AllowAnonymous]
    public async Task<IActionResult> VnpayReturn()
    {
        var result =
            await _paymentService.HandleVnpayReturnAsync(Request.Query);

        if (!result.Success)
        {
            return Redirect("myapp://payment-fail");
        }

        if (result.Data?.Status == "Success")
        {
            return Redirect(
                "myapp://payment-success"
                + "?orderId=" + result.Data.OrderId
                + "&amount=" + result.Data.Amount
                + "&method=" + result.Data.PaymentMethod
                + "&status=" + result.Data.Status
            );
        }

        if (result.Data?.Status == "Canceled")
        {
            return Redirect("myapp://payment-canceled");
        }

        return Redirect("myapp://payment-fail");
    }
}