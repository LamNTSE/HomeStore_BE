using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStore.Domain.Entities;

[Table("Vouchers")]
public partial class Voucher
{
    [Key]
    public int VoucherId { get; set; }

    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Percent | Fixed</summary>
    [MaxLength(10)]
    public string DiscountType { get; set; } = "Percent";

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountValue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinOrderValue { get; set; } = 0;

    public int MaxUsageCount { get; set; } = 1;

    public int UsedCount { get; set; } = 0;

    /// <summary>Date from which the voucher is valid.</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>Date after which the voucher expires.</summary>
    public DateTime? ExpiryDate { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
