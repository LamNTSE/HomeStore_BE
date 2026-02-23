using HomeStore.Domain.DTOs.Carts;
using HomeStore.Domain.DTOs.Common;

namespace HomeStore.Domain.Interfaces.Services;

public interface ICartService
{
    Task<ApiResponse<CartDto>> GetCartAsync(int userId);
    Task<ApiResponse<CartDto>> AddToCartAsync(int userId, AddToCartRequest request);
    Task<ApiResponse<CartDto>> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemRequest request);
    Task<ApiResponse<bool>> RemoveCartItemAsync(int userId, int cartItemId);
    Task<ApiResponse<bool>> ClearCartAsync(int userId);
}
