using AutoMapper;
using HomeStore.Domain.DTOs.Chat;
using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;

namespace HomeStore.BLL.Services;

public class ChatService : IChatService
{
    private readonly IMessageRepository _messageRepo;
    private readonly IMapper _mapper;

    public ChatService(IMessageRepository messageRepo, IMapper mapper)
    {
        _messageRepo = messageRepo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<MessageDto>> SendMessageAsync(int senderId, SendMessageRequest request)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            Content = request.Content
        };

        await _messageRepo.CreateAsync(message);
        return ApiResponse<MessageDto>.Ok(_mapper.Map<MessageDto>(message), "Message sent.");
    }

    public async Task<ApiResponse<List<MessageDto>>> GetConversationAsync(int userId, int otherUserId)
    {
        var messages = await _messageRepo.GetConversationAsync(userId, otherUserId);
        return ApiResponse<List<MessageDto>>.Ok(_mapper.Map<List<MessageDto>>(messages));
    }

    public async Task<ApiResponse<List<MessageDto>>> GetUserMessagesAsync(int userId)
    {
        var messages = await _messageRepo.GetUserMessagesAsync(userId);
        return ApiResponse<List<MessageDto>>.Ok(_mapper.Map<List<MessageDto>>(messages));
    }
}
