using HomeStore.Domain.Entities;

namespace HomeStore.Domain.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment?> GetByOrderIdAsync(int orderId);
    Task UpdateAsync(Payment payment);
}
