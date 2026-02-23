namespace HomeStore.Domain.DTOs.Payments;

public class PaymentResponse
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PaymentUrl { get; set; } // VNPay redirect URL
    public string? TransactionId { get; set; }
}
