using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class StoreLocationRepository : IStoreLocationRepository
{
    private readonly HomeStoreV2Context _context;

    public StoreLocationRepository(HomeStoreV2Context context) => _context = context;

    public async Task<List<StoreLocation>> GetAllAsync()
        => await _context.StoreLocations.Where(s => s.IsActive).ToListAsync();

    public async Task<StoreLocation?> GetByIdAsync(int locationId)
        => await _context.StoreLocations.FindAsync(locationId);

    public async Task<StoreLocation> CreateAsync(StoreLocation location)
    {
        _context.StoreLocations.Add(location);
        await _context.SaveChangesAsync();
        return location;
    }

    public async Task UpdateAsync(StoreLocation location)
    {
        _context.StoreLocations.Update(location);
        await _context.SaveChangesAsync();
    }
}
