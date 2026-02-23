using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly HomeStoreV2Context _context;

    public CategoryRepository(HomeStoreV2Context context) => _context = context;

    public async Task<List<Category>> GetAllAsync()
        => await _context.Categories.ToListAsync();

    public async Task<Category?> GetByIdAsync(int categoryId)
        => await _context.Categories.FindAsync(categoryId);

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int categoryId)
    {
        var cat = await _context.Categories.FindAsync(categoryId);
        if (cat != null)
        {
            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
        }
    }
}
