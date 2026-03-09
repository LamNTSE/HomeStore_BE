using AutoMapper;
using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Orders;
using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;

namespace HomeStore.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly ICartRepository _cartRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepo, ICartRepository cartRepo, IPaymentRepository paymentRepo, IMapper mapper)
    {
        _orderRepo = orderRepo;
        _cartRepo = cartRepo;
        _paymentRepo = paymentRepo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<OrderDto>> CreateOrderAsync(int userId, CreateOrderRequest request)
    {
        var cart = await _cartRepo.GetByUserIdAsync(userId);
        if (cart == null || !cart.CartItems.Any())
            return ApiResponse<OrderDto>.Fail("Cart is empty.");

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = request.ShippingAddress,
            Phone = request.Phone,
            ReceiverName = request.ReceiverName,
            TotalAmount = cart.CartItems.Sum(ci => ci.Product!.Price * ci.Quantity),
            Status = "Pending",
            OrderItems = cart.CartItems.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product!.Price
            }).ToList()
        };

        await _orderRepo.CreateAsync(order);

        // Create payment record
        var payment = new Payment
        {
            OrderId = order.OrderId,
            PaymentMethod = request.PaymentMethod,
            Amount = order.TotalAmount,
            Status = request.PaymentMethod == "COD" ? "Pending" : "Pending"
        };
        await _paymentRepo.CreateAsync(payment);

        // Clear cart
        await _cartRepo.ClearCartAsync(cart.CartId);

        var created = await _orderRepo.GetByIdAsync(order.OrderId);
        return ApiResponse<OrderDto>.Ok(_mapper.Map<OrderDto>(created!), "Order created.");
    }

    public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int userId, int orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null || order.UserId != userId)
            return ApiResponse<OrderDto>.Fail("Order not found.");
        return ApiResponse<OrderDto>.Ok(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResponse<List<OrderDto>>> GetUserOrdersAsync(int userId)
    {
        var orders = await _orderRepo.GetByUserIdAsync(userId);
        return ApiResponse<List<OrderDto>>.Ok(_mapper.Map<List<OrderDto>>(orders));
    }

    public async Task<ApiResponse<List<OrderDto>>> GetAllOrdersAsync()
    {
        var orders = await _orderRepo.GetAllAsync();
        return ApiResponse<List<OrderDto>>.Ok(_mapper.Map<List<OrderDto>>(orders));
    }

    public async Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) return ApiResponse<OrderDto>.Fail("Order not found.");

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepo.UpdateAsync(order);
        return ApiResponse<OrderDto>.Ok(_mapper.Map<OrderDto>(order), "Order status updated.");
    }

    public async Task<ApiResponse<OrderDto>> CancelOrderAsync(int userId, int orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null || order.UserId != userId)
            return ApiResponse<OrderDto>.Fail("Order not found.");

        if (order.Status == "Delivered" || order.Status == "Cancelled")
            return ApiResponse<OrderDto>.Fail("Cannot cancel this order.");

        order.Status = "Cancelled";
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepo.UpdateAsync(order);
        return ApiResponse<OrderDto>.Ok(_mapper.Map<OrderDto>(order), "Order cancelled.");
    }

    public async Task<ApiResponse<OrderDto>> ConfirmDeliveryAsync(int userId, int orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null || order.UserId != userId)
            return ApiResponse<OrderDto>.Fail("Order not found.");

        if (order.Status != "Shipping")
            return ApiResponse<OrderDto>.Fail("Order is not in Shipping status.");

        order.Status = "Delivered";
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepo.UpdateAsync(order);
        return ApiResponse<OrderDto>.Ok(_mapper.Map<OrderDto>(order), "Delivery confirmed.");
    }
}
