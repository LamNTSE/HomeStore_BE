using HomeStore.Domain.DTOs.Products;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "02. Products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService) => _productService = productService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _productService.GetAllProductsAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var result = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        var result = await _productService.SearchProductsAsync(keyword);
        return Ok(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _productService.GetAllCategoriesAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var result = await _productService.CreateProductAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.ProductId }, result) : BadRequest(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        var result = await _productService.UpdateProductAsync(id, request);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("sold")]
    public async Task<IActionResult> GetProductSold()
    {
        var result = await _productService.GetProductSoldAsync();
        return Ok(result);
    }
}
