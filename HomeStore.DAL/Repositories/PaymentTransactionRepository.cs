using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class PaymentTransactionRepository : IPaymentTransactionRepository
{
    private readonly HomeStoreV2Context _context;

    public PaymentTransactionRepository(HomeStoreV2Context context)
        => _context = context;

    public async Task<PaymentTransaction> CreateAsync(PaymentTransaction transaction)
    {
        _context.PaymentTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }
}