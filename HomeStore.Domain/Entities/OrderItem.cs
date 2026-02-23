using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStore.Domain.Entities;

[Table("OrderItems")]
public partial class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    // Navigation
    [ForeignKey("OrderId")]
    public virtual Order? Order { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
