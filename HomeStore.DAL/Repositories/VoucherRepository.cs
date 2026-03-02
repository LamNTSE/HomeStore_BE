using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class VoucherRepository : IVoucherRepository
{
    private readonly HomeStoreV2Context _context;

    public VoucherRepository(HomeStoreV2Context context) => _context = context;

    public async Task<List<Voucher>> GetAllAsync()
        => await _context.Vouchers.OrderByDescending(v => v.CreatedAt).ToListAsync();

    public async Task<Voucher?> GetByIdAsync(int voucherId)
        => await _context.Vouchers.FindAsync(voucherId);

    public async Task<Voucher?> GetByCodeAsync(string code)
        => await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == code);

    public async Task<Voucher> CreateAsync(Voucher voucher)
    {
        _context.Vouchers.Add(voucher);
        await _context.SaveChangesAsync();
        return voucher;
    }

    public async Task UpdateAsync(Voucher voucher)
    {
        _context.Vouchers.Update(voucher);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int voucherId)
    {
        var voucher = await _context.Vouchers.FindAsync(voucherId);
        if (voucher != null)
        {
            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();
        }
    }
}
