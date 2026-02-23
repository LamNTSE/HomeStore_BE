using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Products;

namespace HomeStore.Domain.Interfaces.Services;

public interface IProductService
{
    Task<ApiResponse<List<ProductDto>>> GetAllProductsAsync();
    Task<ApiResponse<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId);
    Task<ApiResponse<ProductDto>> GetProductByIdAsync(int productId);
    Task<ApiResponse<List<ProductDto>>> SearchProductsAsync(string keyword);
    Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductRequest request);
    Task<ApiResponse<ProductDto>> UpdateProductAsync(int productId, UpdateProductRequest request);
    Task<ApiResponse<bool>> DeleteProductAsync(int productId);
    Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync();
}
