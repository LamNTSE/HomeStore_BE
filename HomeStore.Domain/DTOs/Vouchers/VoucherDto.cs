namespace HomeStore.Domain.DTOs.Vouchers;

public class VoucherDto
{
    public int VoucherId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal MinOrderValue { get; set; }
    public int MaxUsageCount { get; set; }
    public int UsedCount { get; set; }
    /// <summary>Số lượng voucher còn lại có thể sử dụng.</summary>
    public int RemainingCount => MaxUsageCount - UsedCount;
    public DateTime? StartDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
