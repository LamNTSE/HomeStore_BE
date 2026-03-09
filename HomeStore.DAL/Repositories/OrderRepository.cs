using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly HomeStoreV2Context _context;

    public OrderRepository(HomeStoreV2Context context) => _context = context;

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> GetByIdAsync(int orderId)
        => await _context.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

    public async Task<List<Order>> GetByUserIdAsync(int userId)
        => await _context.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<List<Order>> GetAllAsync()
        => await _context.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
