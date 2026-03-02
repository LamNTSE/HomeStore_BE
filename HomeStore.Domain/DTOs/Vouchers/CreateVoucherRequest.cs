namespace HomeStore.Domain.DTOs.Vouchers;

public class CreateVoucherRequest
{
    public string Code { get; set; } = string.Empty;
    /// <summary>Percent | Fixed</summary>
    public string DiscountType { get; set; } = "Percent";
    public decimal DiscountValue { get; set; }
    public decimal MinOrderValue { get; set; } = 0;
    public int MaxUsageCount { get; set; } = 1;
    public DateTime? StartDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
