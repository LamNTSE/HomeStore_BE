using AutoMapper;
using HomeStore.Domain.DTOs.Carts;
using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;

namespace HomeStore.BLL.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly IProductRepository _productRepo;
    private readonly IMapper _mapper;

    public CartService(ICartRepository cartRepo, IProductRepository productRepo, IMapper mapper)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CartDto>> GetCartAsync(int userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return ApiResponse<CartDto>.Ok(_mapper.Map<CartDto>(cart));
    }

    public async Task<ApiResponse<CartDto>> AddToCartAsync(int userId, AddToCartRequest request)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId);
        if (product == null) return ApiResponse<CartDto>.Fail("Product not found.");

        var cart = await GetOrCreateCartAsync(userId);
        var existingItem = await _cartRepo.GetCartItemAsync(cart.CartId, request.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
            await _cartRepo.UpdateItemAsync(existingItem);
        }
        else
        {
            var item = new CartItem
            {
                CartId = cart.CartId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
            await _cartRepo.AddItemAsync(item);
        }

        cart = await _cartRepo.GetByUserIdAsync(userId);
        return ApiResponse<CartDto>.Ok(_mapper.Map<CartDto>(cart!), "Added to cart.");
    }

    public async Task<ApiResponse<CartDto>> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemRequest request)
    {
        var cartItem = await _cartRepo.GetCartItemByIdAsync(cartItemId);
        if (cartItem == null) return ApiResponse<CartDto>.Fail("Cart item not found.");

        cartItem.Quantity = request.Quantity;
        await _cartRepo.UpdateItemAsync(cartItem);

        var cart = await _cartRepo.GetByUserIdAsync(userId);
        return ApiResponse<CartDto>.Ok(_mapper.Map<CartDto>(cart!));
    }

    public async Task<ApiResponse<bool>> RemoveCartItemAsync(int userId, int cartItemId)
    {
        await _cartRepo.RemoveItemAsync(cartItemId);
        return ApiResponse<bool>.Ok(true, "Item removed.");
    }

    public async Task<ApiResponse<bool>> ClearCartAsync(int userId)
    {
        var cart = await _cartRepo.GetByUserIdAsync(userId);
        if (cart != null)
            await _cartRepo.ClearCartAsync(cart.CartId);
        return ApiResponse<bool>.Ok(true, "Cart cleared.");
    }

    private async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await _cartRepo.GetByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            await _cartRepo.CreateAsync(cart);
            cart = await _cartRepo.GetByUserIdAsync(userId);
        }
        return cart!;
    }
}
