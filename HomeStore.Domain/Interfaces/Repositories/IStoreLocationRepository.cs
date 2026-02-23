using HomeStore.Domain.Entities;

namespace HomeStore.Domain.Interfaces.Repositories;

public interface IStoreLocationRepository
{
    Task<List<StoreLocation>> GetAllAsync();
    Task<StoreLocation?> GetByIdAsync(int locationId);
    Task<StoreLocation> CreateAsync(StoreLocation location);
    Task UpdateAsync(StoreLocation location);
}
