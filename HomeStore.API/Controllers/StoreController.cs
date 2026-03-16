using HomeStore.Domain.DTOs.Store;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "07. Store")]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService) => _storeService = storeService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] double? lat, [FromQuery] double? lng)
    {
        var result = await _storeService.GetAllStoresAsync(lat, lng);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _storeService.GetStoreByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StoreLocationDto dto)
    {
        var result = await _storeService.CreateStoreAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] StoreLocationDto dto)
    {
        var result = await _storeService.UpdateStoreAsync(id, dto);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
