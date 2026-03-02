namespace HomeStore.Domain.DTOs.Vouchers;

public class UpdateVoucherRequest
{
    public string? Code { get; set; }
    /// <summary>Percent | Fixed</summary>
    public string? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal? MinOrderValue { get; set; }
    public int? MaxUsageCount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool? IsActive { get; set; }
}
