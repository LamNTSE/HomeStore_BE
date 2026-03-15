using AutoMapper;
using HomeStore.DAL.Repositories;
using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Products;
using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;

namespace HomeStore.BLL.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IMapper _mapper;
 

    public ProductService(IProductRepository productRepo, ICategoryRepository categoryRepo, IMapper mapper)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<ProductDto>>> GetAllProductsAsync()
    {
        var products = await _productRepo.GetAllAsync();
        return ApiResponse<List<ProductDto>>.Ok(_mapper.Map<List<ProductDto>>(products));
    }

    public async Task<ApiResponse<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepo.GetByCategoryAsync(categoryId);
        return ApiResponse<List<ProductDto>>.Ok(_mapper.Map<List<ProductDto>>(products));
    }

    public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int productId)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null) return ApiResponse<ProductDto>.Fail("Product not found.");
        return ApiResponse<ProductDto>.Ok(_mapper.Map<ProductDto>(product));
    }

    public async Task<ApiResponse<List<ProductDto>>> SearchProductsAsync(string keyword)
    {
        var products = await _productRepo.SearchAsync(keyword);
        return ApiResponse<List<ProductDto>>.Ok(_mapper.Map<List<ProductDto>>(products));
    }

    public async Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductRequest request)
    {
        var product = _mapper.Map<Product>(request);
        await _productRepo.CreateAsync(product);
        var created = await _productRepo.GetByIdAsync(product.ProductId);
        return ApiResponse<ProductDto>.Ok(_mapper.Map<ProductDto>(created!), "Product created.");
    }

    public async Task<ApiResponse<ProductDto>> UpdateProductAsync(int productId, UpdateProductRequest request)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null) return ApiResponse<ProductDto>.Fail("Product not found.");

        if (request.ProductName != null) product.ProductName = request.ProductName;
        if (request.Description != null) product.Description = request.Description;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (request.StockQuantity.HasValue) product.StockQuantity = request.StockQuantity.Value;
        if (request.ImageUrl != null) product.ImageUrl = request.ImageUrl;
        if (request.CategoryId.HasValue) product.CategoryId = request.CategoryId.Value;
        if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;

        await _productRepo.UpdateAsync(product);
        return ApiResponse<ProductDto>.Ok(_mapper.Map<ProductDto>(product), "Product updated.");
    }

    public async Task<ApiResponse<bool>> DeleteProductAsync(int productId)
    {
        await _productRepo.DeleteAsync(productId);
        return ApiResponse<bool>.Ok(true, "Product deleted.");
    }

    public async Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepo.GetAllAsync();
        return ApiResponse<List<CategoryDto>>.Ok(_mapper.Map<List<CategoryDto>>(categories));
    }

    public async Task<ApiResponse<List<ProductSoldDto>>> GetProductSoldAsync()
    {
        var result = await _productRepo.GetProductSoldAsync();

        return ApiResponse<List<ProductSoldDto>>.Ok(result);
    }
}
