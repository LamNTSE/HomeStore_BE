namespace HomeStore.Domain.DTOs.Carts;

public class CartDto
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(i => i.SubTotal);
}
