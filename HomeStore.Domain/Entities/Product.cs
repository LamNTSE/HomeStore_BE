using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStore.Domain.Entities;

[Table("Products")]
public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required, MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
