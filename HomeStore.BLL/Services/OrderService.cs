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
    private readonly IVoucherRepository _voucherRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepo;
    

    public OrderService(
    IOrderRepository orderRepo,
    ICartRepository cartRepo,
    IPaymentRepository paymentRepo,
    IVoucherRepository voucherRepo,
    IProductRepository productRepo,
    
    IMapper mapper)
    {
        _orderRepo = orderRepo;
        _cartRepo = cartRepo;
        _paymentRepo = paymentRepo;
        _voucherRepo = voucherRepo;
        _productRepo = productRepo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<OrderDto>> CreateOrderAsync(int userId, CreateOrderRequest request)
    {
        var cart = await _cartRepo.GetByUserIdAsync(userId);

        if (cart == null || !cart.CartItems.Any())
            return ApiResponse<OrderDto>.Fail("Cart is empty.");

        // Tổng tiền gốc
        var totalAmount = cart.CartItems.Sum(ci => ci.Product!.Price * ci.Quantity);

        decimal discount = 0;
        Voucher? voucher = null;

        if (request.VoucherId.HasValue)
        {
            voucher = await _voucherRepo.GetByIdAsync(request.VoucherId.Value);

            if (voucher == null || !voucher.IsActive)
                return ApiResponse<OrderDto>.Fail("Voucher is invalid.");

            if (voucher.StartDate.HasValue && voucher.StartDate > DateTime.UtcNow)
                return ApiResponse<OrderDto>.Fail("Voucher not started yet.");

            if (voucher.ExpiryDate.HasValue && voucher.ExpiryDate < DateTime.UtcNow)
                return ApiResponse<OrderDto>.Fail("Voucher expired.");

            if (voucher.UsedCount >= voucher.MaxUsageCount)
                return ApiResponse<OrderDto>.Fail("Voucher usage limit reached.");

            if (totalAmount < voucher.MinOrderValue)
                return ApiResponse<OrderDto>.Fail($"Minimum order is {voucher.MinOrderValue}");

            // Tính discount
            if (voucher.DiscountType == "Percent")
            {
                discount = totalAmount * voucher.DiscountValue / 100;
            }
            else if (voucher.DiscountType == "Fixed")
            {
                discount = voucher.DiscountValue;

                // không cho giảm quá tổng tiền
                discount = Math.Min(discount, totalAmount);
            }

            // tăng số lần sử dụng
            voucher.UsedCount++;
            await _voucherRepo.UpdateAsync(voucher);
        }

        var finalAmount = totalAmount - discount;

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = request.ShippingAddress,
            Phone = request.Phone,
            ReceiverName = request.ReceiverName,

            TotalAmount = finalAmount,
            DiscountAmount = discount,
            VoucherId = request.VoucherId,

            Status = "Pending",
            CreatedAt = DateTime.UtcNow,

            OrderItems = cart.CartItems.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product!.Price
            }).ToList()
        };

        await _orderRepo.CreateAsync(order);

        var payment = new Payment
        {
            OrderId = order.OrderId,
            PaymentMethod = request.PaymentMethod,
            Amount = finalAmount,
            Status = "Pending"
        };

        await _paymentRepo.CreateAsync(payment);

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

        var oldStatus = order.Status;

        order.Status = "Cancelled";
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepo.UpdateAsync(order);

        // 🔥 Restore stock nếu cancel khi đang Shipping
        if (oldStatus == "Shipping")
        {
            var orderItems = order.OrderItems;

            foreach (var item in orderItems)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);

                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    await _productRepo.UpdateAsync(product);
                }
            }
        }

        var payment = await _paymentRepo.GetByOrderIdAsync(orderId);

        if (payment != null)
        {
            payment.Status = "Cancelled";
            await _paymentRepo.UpdateAsync(payment);
        }

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

        // 🔥 UPDATE PAYMENT FOR COD
        var payment = await _paymentRepo.GetByOrderIdAsync(orderId);

        if (payment != null && payment.PaymentMethod == "COD")
        {
            payment.Status = "Success";
            await _paymentRepo.UpdateAsync(payment);
        }

        return ApiResponse<OrderDto>.Ok(_mapper.Map<OrderDto>(order), "Delivery confirmed.");
    }

}
