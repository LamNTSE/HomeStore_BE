namespace HomeStore.Domain.DTOs.Orders;

public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
    public string? Phone { get; set; }
    public string? ReceiverName { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
}
