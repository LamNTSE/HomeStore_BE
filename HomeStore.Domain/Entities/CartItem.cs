using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStore.Domain.Entities;

[Table("CartItems")]
public partial class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    // Navigation
    [ForeignKey("CartId")]
    public virtual Cart? Cart { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
