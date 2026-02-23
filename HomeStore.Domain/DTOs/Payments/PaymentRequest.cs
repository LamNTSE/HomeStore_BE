namespace HomeStore.Domain.DTOs.Payments;

public class PaymentRequest
{
    public int OrderId { get; set; }
    public string PaymentMethod { get; set; } = "COD"; // COD | VNPay
}
