namespace HomeStore.Domain.DTOs.Products;

public class UpdateProductRequest
{
    public string? ProductName { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsActive { get; set; }
}
