using HomeStore.Domain.DTOs.Products;
using HomeStore.Domain.Entities;

namespace HomeStore.Domain.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetByCategoryAsync(int categoryId);
    Task<Product?> GetByIdAsync(int productId);
    Task<List<Product>> SearchAsync(string keyword);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int productId);
    Task<List<ProductSoldDto>> GetProductSoldAsync();
}
