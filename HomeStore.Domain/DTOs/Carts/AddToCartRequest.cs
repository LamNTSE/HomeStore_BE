namespace HomeStore.Domain.DTOs.Carts;

public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}
