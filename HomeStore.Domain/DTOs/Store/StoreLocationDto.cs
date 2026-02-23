namespace HomeStore.Domain.DTOs.Store;

public class StoreLocationDto
{
    public int LocationId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public double? DistanceKm { get; set; } // calculated from user location
}
