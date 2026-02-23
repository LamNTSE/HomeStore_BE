using HomeStore.Domain.Entities;

namespace HomeStore.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int categoryId);
    Task<Category> CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int categoryId);
}
