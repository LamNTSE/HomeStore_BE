using HomeStore.Domain.DTOs.Chat;
using HomeStore.Domain.DTOs.Common;

namespace HomeStore.Domain.Interfaces.Services;

public interface IChatService
{
    Task<ApiResponse<MessageDto>> SendMessageAsync(int senderId, SendMessageRequest request);
    Task<ApiResponse<List<MessageDto>>> GetConversationAsync(int userId, int otherUserId);
    Task<ApiResponse<List<MessageDto>>> GetUserMessagesAsync(int userId);
    Task<ApiResponse<List<ChatPartnerDto>>> GetConversationPartnersAsync(int userId);
    Task<ApiResponse<ChatPartnerDto>> GetAdminUserAsync();
}
