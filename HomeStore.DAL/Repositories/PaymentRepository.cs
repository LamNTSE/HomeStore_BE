using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly HomeStoreV2Context _context;

    public PaymentRepository(HomeStoreV2Context context) => _context = context;

    public async Task<Payment> CreateAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<Payment?> GetByOrderIdAsync(int orderId)
        => await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }
}
