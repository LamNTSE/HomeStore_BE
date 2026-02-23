using HomeStore.Domain.Entities;

namespace HomeStore.Domain.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart> CreateAsync(Cart cart);
    Task<CartItem?> GetCartItemAsync(int cartId, int productId);
    Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
    Task AddItemAsync(CartItem item);
    Task UpdateItemAsync(CartItem item);
    Task RemoveItemAsync(int cartItemId);
    Task ClearCartAsync(int cartId);
}
