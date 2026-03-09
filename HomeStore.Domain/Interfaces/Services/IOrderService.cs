using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Orders;

namespace HomeStore.Domain.Interfaces.Services;

public interface IOrderService
{
    Task<ApiResponse<OrderDto>> CreateOrderAsync(int userId, CreateOrderRequest request);
    Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int userId, int orderId);
    Task<ApiResponse<List<OrderDto>>> GetUserOrdersAsync(int userId);
    Task<ApiResponse<List<OrderDto>>> GetAllOrdersAsync();
    Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(int orderId, string status);
    Task<ApiResponse<OrderDto>> CancelOrderAsync(int userId, int orderId);
    Task<ApiResponse<OrderDto>> ConfirmDeliveryAsync(int userId, int orderId);
}
