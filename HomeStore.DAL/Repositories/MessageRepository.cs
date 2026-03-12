using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly HomeStoreV2Context _context;

    public MessageRepository(HomeStoreV2Context context) => _context = context;

    public async Task<Message> CreateAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<List<Message>> GetConversationAsync(int userId1, int userId2)
        => await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

    public async Task<List<Message>> GetUserMessagesAsync(int userId)
        => await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

    public async Task MarkAsReadAsync(int messageId)
    {
        var msg = await _context.Messages.FindAsync(messageId);
        if (msg != null)
        {
            msg.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<(User Partner, Message LastMsg)>> GetConversationPartnersAsync(int userId)
    {
        // Get all messages involving this user
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        // Group by the OTHER user and pick the latest message
        var partners = messages
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g =>
            {
                var lastMsg = g.First(); // already sorted desc
                var partner = lastMsg.SenderId == userId ? lastMsg.Receiver! : lastMsg.Sender!;
                return (Partner: partner, LastMsg: lastMsg);
            })
            .OrderByDescending(x => x.LastMsg.SentAt)
            .ToList();

        return partners;
    }

    public async Task<User?> FindAdminUserAsync()
        => await _context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
}
