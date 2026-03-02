using HomeStore.Domain.Entities;

namespace HomeStore.Domain.Interfaces.Repositories;

public interface IVoucherRepository
{
    Task<List<Voucher>> GetAllAsync();
    Task<Voucher?> GetByIdAsync(int voucherId);
    Task<Voucher?> GetByCodeAsync(string code);
    Task<Voucher> CreateAsync(Voucher voucher);
    Task UpdateAsync(Voucher voucher);
    Task DeleteAsync(int voucherId);
}
