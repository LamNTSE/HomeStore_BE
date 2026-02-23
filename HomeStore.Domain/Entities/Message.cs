using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStore.Domain.Entities;

[Table("Messages")]
public partial class Message
{
    [Key]
    public int MessageId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    [Required, MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey("SenderId")]
    public virtual User? Sender { get; set; }

    [ForeignKey("ReceiverId")]
    public virtual User? Receiver { get; set; }
}
