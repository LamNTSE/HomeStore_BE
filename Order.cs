public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Phone { get; set; }
    public string? ReceiverName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? VoucherId { get; set; } // <-- Add this property to fix CS0117

    public virtual ICollection<OrderItem> OrderItems { get; set; }
}