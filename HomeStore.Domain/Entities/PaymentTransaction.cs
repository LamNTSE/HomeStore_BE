using HomeStore.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PaymentTransactions")]
public class PaymentTransaction
{
    [Key]
    public int TransactionId { get; set; }

    public int PaymentId { get; set; }

    public string Gateway { get; set; } = "VNPay";

    public string GatewayTransactionId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string ResponseCode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string RawData { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Payment Payment { get; set; } = null!;
}
