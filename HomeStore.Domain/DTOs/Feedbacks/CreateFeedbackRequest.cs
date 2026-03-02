namespace HomeStore.Domain.DTOs.Feedbacks;

public class CreateFeedbackRequest
{
    public int ProductId { get; set; }
    /// <summary>Rating from 1 to 5.</summary>
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
