using System.ComponentModel.DataAnnotations;

namespace HomeStore.Domain.DTOs.Feedbacks;

public class AdminReplyFeedbackRequest
{
    [Required]
    [MaxLength(2000)]
    public string AdminReply { get; set; } = string.Empty;
}
