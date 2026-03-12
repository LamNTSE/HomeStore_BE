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

        // Re-fetch with navigation for proper mapping
        var messages = await _messageRepo.GetConversationAsync(senderId, request.ReceiverId);
        var sent = messages.LastOrDefault(m => m.MessageId == message.MessageId) ?? message;

        var dto = new MessageDto
        {
            MessageId = sent.MessageId,
            SenderId = sent.SenderId,
            SenderName = sent.Sender?.FullName ?? "",
            SenderAvatarUrl = sent.Sender?.AvatarUrl,
            ReceiverId = sent.ReceiverId,
            ReceiverName = sent.Receiver?.FullName ?? "",
            ReceiverAvatarUrl = sent.Receiver?.AvatarUrl,
            Content = sent.Content,
            IsRead = sent.IsRead,
            SentAt = sent.SentAt
        };

        return ApiResponse<MessageDto>.Ok(dto, "Message sent.");
    }

    public async Task<ApiResponse<List<MessageDto>>> GetConversationAsync(int userId, int otherUserId)
    {
        var messages = await _messageRepo.GetConversationAsync(userId, otherUserId);
        var dtos = messages.Select(m => new MessageDto
        {
            MessageId = m.MessageId,
            SenderId = m.SenderId,
            SenderName = m.Sender?.FullName ?? "",
            SenderAvatarUrl = m.Sender?.AvatarUrl,
            ReceiverId = m.ReceiverId,
            ReceiverName = m.Receiver?.FullName ?? "",
            ReceiverAvatarUrl = m.Receiver?.AvatarUrl,
            Content = m.Content,
            IsRead = m.IsRead,
            SentAt = m.SentAt
        }).ToList();

        return ApiResponse<List<MessageDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<List<MessageDto>>> GetUserMessagesAsync(int userId)
    {
        var messages = await _messageRepo.GetUserMessagesAsync(userId);
        var dtos = messages.Select(m => new MessageDto
        {
            MessageId = m.MessageId,
            SenderId = m.SenderId,
            SenderName = m.Sender?.FullName ?? "",
            SenderAvatarUrl = m.Sender?.AvatarUrl,
            ReceiverId = m.ReceiverId,
            ReceiverName = m.Receiver?.FullName ?? "",
            ReceiverAvatarUrl = m.Receiver?.AvatarUrl,
            Content = m.Content,
            IsRead = m.IsRead,
            SentAt = m.SentAt
        }).ToList();

        return ApiResponse<List<MessageDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<List<ChatPartnerDto>>> GetConversationPartnersAsync(int userId)
    {
        var partners = await _messageRepo.GetConversationPartnersAsync(userId);
        var dtos = partners.Select(p => new ChatPartnerDto
        {
            UserId = p.Partner.UserId,
            FullName = p.Partner.FullName,
            AvatarUrl = p.Partner.AvatarUrl,
            LastMessage = p.LastMsg.Content,
            LastMessageTime = p.LastMsg.SentAt
        }).ToList();

        return ApiResponse<List<ChatPartnerDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<ChatPartnerDto>> GetAdminUserAsync()
    {
        var admin = await _messageRepo.FindAdminUserAsync();
        if (admin == null)
            return ApiResponse<ChatPartnerDto>.Fail("No admin user found.");

        var dto = new ChatPartnerDto
        {
            UserId = admin.UserId,
            FullName = admin.FullName,
            AvatarUrl = admin.AvatarUrl
        };

        return ApiResponse<ChatPartnerDto>.Ok(dto);
    }
}
