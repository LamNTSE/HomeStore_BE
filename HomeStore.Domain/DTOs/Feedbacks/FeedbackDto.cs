namespace HomeStore.Domain.DTOs.Feedbacks;

public class FeedbackDto
{
    public int FeedbackId { get; set; }
    public int UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? AdminReply { get; set; }
    public DateTime? AdminReplyAt { get; set; }
}
