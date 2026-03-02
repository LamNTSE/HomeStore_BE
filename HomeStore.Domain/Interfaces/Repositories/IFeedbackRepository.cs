using HomeStore.Domain.Entities;

namespace HomeStore.Domain.Interfaces.Repositories;

public interface IFeedbackRepository
{
    Task<List<Feedback>> GetAllAsync();
    Task<List<Feedback>> GetByProductIdAsync(int productId);
    Task<List<Feedback>> GetByUserIdAsync(int userId);
    Task<Feedback?> GetByIdAsync(int feedbackId);
    Task<Feedback?> GetByUserAndProductAsync(int userId, int productId);
    Task<Feedback> CreateAsync(Feedback feedback);
    Task UpdateAsync(Feedback feedback);
    Task DeleteAsync(int feedbackId);
}
