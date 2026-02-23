using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class CartRepository : ICartRepository
{
    private readonly HomeStoreV2Context _context;

    public CartRepository(HomeStoreV2Context context) => _context = context;

    public async Task<Cart?> GetByUserIdAsync(int userId)
        => await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task<Cart> CreateAsync(Cart cart)
    {
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        => await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

    public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
        => await _context.CartItems.FindAsync(cartItemId);

    public async Task AddItemAsync(CartItem item)
    {
        _context.CartItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(CartItem item)
    {
        _context.CartItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveItemAsync(int cartItemId)
    {
        var item = await _context.CartItems.FindAsync(cartItemId);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(int cartId)
    {
        var items = await _context.CartItems.Where(ci => ci.CartId == cartId).ToListAsync();
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}
