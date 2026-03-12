using HomeStore.Domain.Entities;

namespace HomeStore.Domain.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<Message> CreateAsync(Message message);
    Task<List<Message>> GetConversationAsync(int userId1, int userId2);
    Task<List<Message>> GetUserMessagesAsync(int userId);
    Task MarkAsReadAsync(int messageId);
    Task<List<(User Partner, Message LastMsg)>> GetConversationPartnersAsync(int userId);
    Task<User?> FindAdminUserAsync();
}
