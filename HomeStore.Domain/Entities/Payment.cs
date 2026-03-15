using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStore.Domain.Entities;

[Table("Payments")]
public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    [MaxLength(50)]
    public string PaymentMethod { get; set; } = "COD"; // COD | VNPay

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending | Completed | Failed

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey("OrderId")]
    public virtual Order? Order { get; set; }

    public ICollection<PaymentTransaction> Transactions { get; set; }
    = new List<PaymentTransaction>();

}
