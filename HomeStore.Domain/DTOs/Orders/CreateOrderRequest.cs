namespace HomeStore.Domain.DTOs.Orders;

public class CreateOrderRequest
{
    public string? ShippingAddress { get; set; }
    public string? Phone { get; set; }
    public string? ReceiverName { get; set; }
    public string PaymentMethod { get; set; } = "COD"; // COD | VNPay

    public int? VoucherId { get; set; }
}
