namespace HomeStore.Domain.DTOs.Chat;

public class MessageDto
{
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public int ReceiverId { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
}
