using HomeStore.Domain.DTOs.Vouchers;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "08. Vouchers")]
public class VouchersController : ControllerBase
{
    private readonly IVoucherService _voucherService;

    public VouchersController(IVoucherService voucherService) => _voucherService = voucherService;

    /// <summary>Admin: get all vouchers</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _voucherService.GetAllVouchersAsync();
        return Ok(result);
    }

    /// <summary>Admin: get voucher by id</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _voucherService.GetVoucherByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Customer/Admin: look up voucher by code to apply at checkout</summary>
    [Authorize]
    [HttpGet("by-code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var result = await _voucherService.GetVoucherByCodeAsync(code);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Admin: create voucher</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVoucherRequest request)
    {
        var result = await _voucherService.CreateVoucherAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.VoucherId }, result) : BadRequest(result);
    }

    /// <summary>Admin: update voucher</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVoucherRequest request)
    {
        var result = await _voucherService.UpdateVoucherAsync(id, request);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Admin: delete voucher</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _voucherService.DeleteVoucherAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Customer: get available vouchers</summary>
    [Authorize(Roles = "Customer,Admin")]
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable()
    {
        var result = await _voucherService.GetAllVouchersAsync();

        if (!result.Success)
            return BadRequest(result);

        // chỉ lấy voucher còn hiệu lực
        var available = result.Data!
            .Where(v =>
                v.IsActive &&
                v.StartDate <= DateTime.UtcNow &&
                v.ExpiryDate >= DateTime.UtcNow)
            .ToList();

        return Ok(new
        {
            success = true,
            data = available
        });
    }
}
