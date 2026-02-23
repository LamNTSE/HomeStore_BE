using HomeStore.BLL.Helpers;
using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Payments;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HomeStore.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IConfiguration _config;

    public PaymentService(IPaymentRepository paymentRepo, IOrderRepository orderRepo, IConfiguration config)
    {
        _paymentRepo = paymentRepo;
        _orderRepo = orderRepo;
        _config = config;
    }

    public async Task<ApiResponse<PaymentResponse>> CreatePaymentAsync(int userId, PaymentRequest request, HttpContext httpContext)
    {
        var order = await _orderRepo.GetByIdAsync(request.OrderId);
        if (order == null || order.UserId != userId)
            return ApiResponse<PaymentResponse>.Fail("Order not found.");

        var payment = await _paymentRepo.GetByOrderIdAsync(request.OrderId);
        if (payment == null)
            return ApiResponse<PaymentResponse>.Fail("Payment not found.");

        if (request.PaymentMethod == "COD")
        {
            payment.PaymentMethod = "COD";
            payment.Status = "Pending";
            await _paymentRepo.UpdateAsync(payment);

            return ApiResponse<PaymentResponse>.Ok(new PaymentResponse
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                PaymentMethod = "COD",
                Amount = payment.Amount,
                Status = "Pending"
            }, "COD payment created.");
        }

        // VNPay
        var vnp = new VnpayHelper();
        var vnpUrl = _config["VNPay:Url"]!;
        var vnpTmnCode = _config["VNPay:TmnCode"]!;
        var vnpHashSecret = _config["VNPay:HashSecret"]!;
        var vnpReturnUrl = _config["VNPay:ReturnUrl"]!;

        vnp.AddRequestData("vnp_Version", "2.1.0");
        vnp.AddRequestData("vnp_Command", "pay");
        vnp.AddRequestData("vnp_TmnCode", vnpTmnCode);
        vnp.AddRequestData("vnp_Amount", ((long)(payment.Amount * 100)).ToString());
        vnp.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        vnp.AddRequestData("vnp_CurrCode", "VND");
        vnp.AddRequestData("vnp_IpAddr", httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
        vnp.AddRequestData("vnp_Locale", "vn");
        vnp.AddRequestData("vnp_OrderInfo", $"Payment for order #{order.OrderId}");
        vnp.AddRequestData("vnp_OrderType", "other");
        vnp.AddRequestData("vnp_ReturnUrl", vnpReturnUrl);
        vnp.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

        var paymentUrl = vnp.CreateRequestUrl(vnpUrl, vnpHashSecret);

        payment.PaymentMethod = "VNPay";
        await _paymentRepo.UpdateAsync(payment);

        return ApiResponse<PaymentResponse>.Ok(new PaymentResponse
        {
            PaymentId = payment.PaymentId,
            OrderId = payment.OrderId,
            PaymentMethod = "VNPay",
            Amount = payment.Amount,
            Status = "Pending",
            PaymentUrl = paymentUrl
        }, "VNPay payment URL generated.");
    }

    public async Task<ApiResponse<PaymentResponse>> HandleVnpayReturnAsync(IQueryCollection queryParams)
    {
        var vnp = new VnpayHelper();
        foreach (var (key, value) in queryParams)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                vnp.AddResponseData(key, value.ToString());
        }

        var vnpHashSecret = _config["VNPay:HashSecret"]!;
        var vnpSecureHash = queryParams["vnp_SecureHash"].ToString();
        var isValid = vnp.ValidateSignature(vnpSecureHash, vnpHashSecret);

        if (!isValid)
            return ApiResponse<PaymentResponse>.Fail("Invalid VNPay signature.");

        var orderId = int.Parse(queryParams["vnp_TxnRef"].ToString());
        var responseCode = queryParams["vnp_ResponseCode"].ToString();
        var transactionId = queryParams["vnp_TransactionNo"].ToString();

        var payment = await _paymentRepo.GetByOrderIdAsync(orderId);
        if (payment == null)
            return ApiResponse<PaymentResponse>.Fail("Payment not found.");

        if (responseCode == "00")
        {
            payment.Status = "Completed";
            payment.TransactionId = transactionId;

            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = "Confirmed";
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepo.UpdateAsync(order);
            }
        }
        else
        {
            payment.Status = "Failed";
        }

        await _paymentRepo.UpdateAsync(payment);

        return ApiResponse<PaymentResponse>.Ok(new PaymentResponse
        {
            PaymentId = payment.PaymentId,
            OrderId = payment.OrderId,
            PaymentMethod = payment.PaymentMethod,
            Amount = payment.Amount,
            Status = payment.Status,
            TransactionId = payment.TransactionId
        });
    }
}
