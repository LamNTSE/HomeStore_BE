using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStore.Domain.Entities;

[Table("StoreLocations")]
public partial class StoreLocation
{
    [Key]
    public int LocationId { get; set; }

    [Required, MaxLength(200)]
    public string StoreName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;
}
