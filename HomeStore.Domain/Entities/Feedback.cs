using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStore.Domain.Entities;

[Table("Feedbacks")]
public partial class Feedback
{
    [Key]
    public int FeedbackId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    /// <summary>Rating from 1 to 5.</summary>
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(2000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(2000)]
    public string? AdminReply { get; set; }

    public DateTime? AdminReplyAt { get; set; }

    // Navigation
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
