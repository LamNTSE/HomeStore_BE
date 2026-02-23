using HomeStore.Domain.Entities;

namespace HomeStore.Domain.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<Order?> GetByIdAsync(int orderId);
    Task<List<Order>> GetByUserIdAsync(int userId);
    Task UpdateAsync(Order order);
}
