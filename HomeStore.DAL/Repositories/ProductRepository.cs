using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly HomeStoreV2Context _context;

    public ProductRepository(HomeStoreV2Context context) => _context = context;

    public async Task<List<Product>> GetAllAsync()
        => await _context.Products.Include(p => p.Category).Where(p => p.IsActive).ToListAsync();

    public async Task<List<Product>> GetByCategoryAsync(int categoryId)
        => await _context.Products.Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.IsActive).ToListAsync();

    public async Task<Product?> GetByIdAsync(int productId)
        => await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == productId);

    public async Task<List<Product>> SearchAsync(string keyword)
        => await _context.Products.Include(p => p.Category)
            .Where(p => p.IsActive && (p.ProductName.Contains(keyword) || (p.Description != null && p.Description.Contains(keyword))))
            .ToListAsync();

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product != null)
        {
            product.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
