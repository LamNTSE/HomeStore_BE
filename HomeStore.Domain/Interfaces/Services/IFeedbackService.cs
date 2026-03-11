using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Feedbacks;

namespace HomeStore.Domain.Interfaces.Services;

public interface IFeedbackService
{
    Task<ApiResponse<List<FeedbackDto>>> GetAllFeedbacksAsync();
    Task<ApiResponse<List<FeedbackDto>>> GetFeedbacksByProductAsync(int productId);
    Task<ApiResponse<List<FeedbackDto>>> GetMyFeedbacksAsync(int userId);
    Task<ApiResponse<FeedbackDto>> GetFeedbackByIdAsync(int feedbackId);
    Task<ApiResponse<FeedbackDto>> CreateFeedbackAsync(int userId, CreateFeedbackRequest request);
    Task<ApiResponse<FeedbackDto>> UpdateFeedbackAsync(int userId, int feedbackId, UpdateFeedbackRequest request);
    Task<ApiResponse<bool>> DeleteFeedbackAsync(int userId, int feedbackId, bool isAdmin);
    Task<ApiResponse<FeedbackDto>> AdminReplyFeedbackAsync(int feedbackId, AdminReplyFeedbackRequest request);
}
