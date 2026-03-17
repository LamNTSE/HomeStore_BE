namespace HomeStore.Domain.DTOs.Feedbacks;

public class CreateFeedbackRequest
{
    public int ProductId { get; set; }
    /// <summary>Rating from 1 to 5.</summary>
    public int Rating { get; set; }

    public int OrderId { get; set; }   // THÊM DÒNG NÀY
    public string? Comment { get; set; }
}
