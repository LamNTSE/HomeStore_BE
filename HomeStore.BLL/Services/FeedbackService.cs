using AutoMapper;
using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Feedbacks;
using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;

namespace HomeStore.BLL.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IMapper _mapper;

    public FeedbackService(
        IFeedbackRepository feedbackRepo,
        IOrderRepository orderRepo,
        IMapper mapper)
    {
        _feedbackRepo = feedbackRepo;
        _orderRepo = orderRepo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<FeedbackDto>>> GetAllFeedbacksAsync()
    {
        var data = await _feedbackRepo.GetAllAsync();
        return ApiResponse<List<FeedbackDto>>.Ok(_mapper.Map<List<FeedbackDto>>(data));
    }

    public async Task<ApiResponse<List<FeedbackDto>>> GetFeedbacksByProductAsync(int productId)
    {
        var data = await _feedbackRepo.GetByProductIdAsync(productId);

        return ApiResponse<List<FeedbackDto>>.Ok(
            _mapper.Map<List<FeedbackDto>>(data)
        );
    }

    public async Task<ApiResponse<List<FeedbackDto>>> GetFeedbacksByProductAndOrderAsync(int productId, int orderId)
    {
        var data = await _feedbackRepo.GetByProductAndOrderAsync(productId, orderId);

        return ApiResponse<List<FeedbackDto>>.Ok(
            _mapper.Map<List<FeedbackDto>>(data)
        );
    }

    public async Task<ApiResponse<List<FeedbackDto>>> GetMyFeedbacksAsync(int userId)
    {
        var data = await _feedbackRepo.GetByUserIdAsync(userId);
        return ApiResponse<List<FeedbackDto>>.Ok(_mapper.Map<List<FeedbackDto>>(data));
    }

    public async Task<ApiResponse<FeedbackDto>> GetFeedbackByIdAsync(int feedbackId)
    {
        var feedback = await _feedbackRepo.GetByIdAsync(feedbackId);

        if (feedback == null)
            return ApiResponse<FeedbackDto>.Fail("Feedback not found");

        return ApiResponse<FeedbackDto>.Ok(_mapper.Map<FeedbackDto>(feedback));
    }

    public async Task<ApiResponse<FeedbackDto>> CreateFeedbackAsync(int userId, CreateFeedbackRequest request)
    {
        if (request.Rating < 1 || request.Rating > 5)
            return ApiResponse<FeedbackDto>.Fail("Rating must be between 1 and 5");

        // ✅ Check order tồn tại
        var order = await _orderRepo.GetByIdAsync(request.OrderId);

        if (order == null)
            return ApiResponse<FeedbackDto>.Fail("Order not found");

        // ✅ Check đã feedback chưa
        var existing = await _feedbackRepo.GetByUserProductAndOrderAsync(
            userId,
            request.ProductId,
            request.OrderId
        );

        if (existing != null)
            return ApiResponse<FeedbackDto>.Fail("Feedback already exists for this order");

        var feedback = new Feedback
        {
            UserId = userId,
            ProductId = request.ProductId,
            OrderId = request.OrderId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _feedbackRepo.CreateAsync(feedback);

        return ApiResponse<FeedbackDto>.Ok(
            _mapper.Map<FeedbackDto>(feedback),
            "Feedback created"
        );
    }

    public async Task<ApiResponse<FeedbackDto>> UpdateFeedbackAsync(int userId, int feedbackId, UpdateFeedbackRequest request)
    {
        var feedback = await _feedbackRepo.GetByIdAsync(feedbackId);

        if (feedback == null)
            return ApiResponse<FeedbackDto>.Fail("Feedback not found");

        if (feedback.UserId != userId)
            return ApiResponse<FeedbackDto>.Fail("You can only edit your own feedback");

        if (request.Rating.HasValue)
            feedback.Rating = request.Rating.Value;

        if (request.Comment != null)
            feedback.Comment = request.Comment;

        feedback.UpdatedAt = DateTime.UtcNow;

        await _feedbackRepo.UpdateAsync(feedback);

        return ApiResponse<FeedbackDto>.Ok(
            _mapper.Map<FeedbackDto>(feedback),
            "Feedback updated"
        );
    }

    public async Task<ApiResponse<bool>> DeleteFeedbackAsync(int userId, int feedbackId, bool isAdmin)
    {
        var feedback = await _feedbackRepo.GetByIdAsync(feedbackId);

        if (feedback == null)
            return ApiResponse<bool>.Fail("Feedback not found");

        if (!isAdmin && feedback.UserId != userId)
            return ApiResponse<bool>.Fail("You can only delete your own feedback");

        await _feedbackRepo.DeleteAsync(feedbackId);

        return ApiResponse<bool>.Ok(true, "Feedback deleted");
    }

    public async Task<ApiResponse<FeedbackDto>> AdminReplyFeedbackAsync(int feedbackId, AdminReplyFeedbackRequest request)
    {
        var feedback = await _feedbackRepo.GetByIdAsync(feedbackId);

        if (feedback == null)
            return ApiResponse<FeedbackDto>.Fail("Feedback not found");

        feedback.AdminReply = request.AdminReply;
        feedback.AdminReplyAt = DateTime.UtcNow;

        await _feedbackRepo.UpdateAsync(feedback);

        return ApiResponse<FeedbackDto>.Ok(
            _mapper.Map<FeedbackDto>(feedback),
            "Reply saved"
        );
    }
}