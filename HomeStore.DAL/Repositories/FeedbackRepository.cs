using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly HomeStoreV2Context _context;

    public FeedbackRepository(HomeStoreV2Context context) => _context = context;

    public async Task<List<Feedback>> GetAllAsync()
        => await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Product)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

    public async Task<List<Feedback>> GetByProductIdAsync(int productId)
        => await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Product)
            .Where(f => f.ProductId == productId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

    public async Task<List<Feedback>> GetByUserIdAsync(int userId)
        => await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Product)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

    public async Task<Feedback?> GetByIdAsync(int feedbackId)
        => await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Product)
            .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);

    // method cũ (có thể giữ lại nếu đang dùng)
    public async Task<Feedback?> GetByUserAndProductAsync(int userId, int productId)
        => await _context.Feedbacks
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

    // ⭐ method mới để tránh ghi đè feedback khi cùng sản phẩm khác đơn
    public async Task<Feedback?> GetByUserProductAndOrderAsync(int userId, int productId, int orderId)
    {
        return await _context.Feedbacks
                .FirstOrDefaultAsync(f =>
                    f.UserId == userId &&
                    f.ProductId == productId &&
                    f.OrderId == orderId);
    }

    public async Task<List<Feedback>> GetByProductAndOrderAsync(int productId, int orderId)
    {
        return await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Product)
            .Where(f => f.ProductId == productId && f.OrderId == orderId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }
    public async Task<Feedback> CreateAsync(Feedback feedback)
    {
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();
        return feedback;
    }

    public async Task UpdateAsync(Feedback feedback)
    {
        _context.Feedbacks.Update(feedback);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int feedbackId)
    {
        var feedback = await _context.Feedbacks.FindAsync(feedbackId);
        if (feedback != null)
        {
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
        }
    }
}
