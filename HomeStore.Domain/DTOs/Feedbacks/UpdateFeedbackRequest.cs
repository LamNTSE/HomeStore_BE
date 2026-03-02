namespace HomeStore.Domain.DTOs.Feedbacks;

public class UpdateFeedbackRequest
{
    /// <summary>Rating from 1 to 5.</summary>
    public int? Rating { get; set; }
    public string? Comment { get; set; }
}
