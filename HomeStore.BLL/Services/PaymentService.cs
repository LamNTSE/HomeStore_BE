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
    private readonly IPaymentTransactionRepository _paymentTransactionRepo;

    public PaymentService(
    IPaymentRepository paymentRepo,
    IOrderRepository orderRepo,
    IPaymentTransactionRepository paymentTransactionRepo,
    IConfiguration config)
    {
        _paymentRepo = paymentRepo;
        _orderRepo = orderRepo;
        _paymentTransactionRepo = paymentTransactionRepo;
        _config = config;
    }

    public async Task<ApiResponse<PaymentResponse>> CreatePaymentAsync(
        int userId,
        PaymentRequest request,
        HttpContext httpContext)
    {
        var order = await _orderRepo.GetByIdAsync(request.OrderId);

        if (order == null || order.UserId != userId)
            return ApiResponse<PaymentResponse>.Fail("Order not found.");

        var payment = await _paymentRepo.GetByOrderIdAsync(request.OrderId);

        if (payment == null)
            return ApiResponse<PaymentResponse>.Fail("Payment not found.");

        // ================= COD =================

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
                Status = payment.Status
            });
        }

        // ================= VNPay =================

        var vnp = new VnpayHelper();

        var vnpUrl = _config["VNPay:Url"]!;
        var vnpTmnCode = _config["VNPay:TmnCode"]!;
        var vnpHashSecret = _config["VNPay:HashSecret"]!;
        var vnpReturnUrl = _config["VNPay:ReturnUrl"]!;

        var createDate = DateTime.Now;
        var expireDate = createDate.AddMinutes(15);

        vnp.AddRequestData("vnp_Version", "2.1.0");
        vnp.AddRequestData("vnp_Command", "pay");
        vnp.AddRequestData("vnp_TmnCode", vnpTmnCode);

        vnp.AddRequestData("vnp_Amount",
            ((long)(payment.Amount * 100)).ToString());

        vnp.AddRequestData("vnp_CreateDate",
            createDate.ToString("yyyyMMddHHmmss"));

        vnp.AddRequestData("vnp_ExpireDate",
            expireDate.ToString("yyyyMMddHHmmss"));

        vnp.AddRequestData("vnp_CurrCode", "VND");

        vnp.AddRequestData("vnp_IpAddr", "127.0.0.1");

        vnp.AddRequestData("vnp_Locale", "vn");

        vnp.AddRequestData("vnp_OrderInfo",
            $"Payment for order #{order.OrderId}");

        vnp.AddRequestData("vnp_OrderType", "other");

        vnp.AddRequestData("vnp_ReturnUrl", vnpReturnUrl);

        vnp.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

        var paymentUrl = vnp.CreateRequestUrl(vnpUrl, vnpHashSecret);

        payment.PaymentMethod = "VNPay";
        payment.Status = "Pending";

        await _paymentRepo.UpdateAsync(payment);

        return ApiResponse<PaymentResponse>.Ok(new PaymentResponse
        {
            PaymentId = payment.PaymentId,
            OrderId = payment.OrderId,
            PaymentMethod = "VNPay",
            Amount = payment.Amount,
            Status = payment.Status,
            PaymentUrl = paymentUrl
        });
    }

    public async Task<ApiResponse<PaymentResponse>> HandleVnpayReturnAsync(
IQueryCollection queryParams)
    {
        try
        {
            var vnp = new VnpayHelper();

            foreach (var (key, value) in queryParams)
            {
                if (!string.IsNullOrEmpty(key)
                    && key.StartsWith("vnp_")
                    && key != "vnp_SecureHash"
                    && key != "vnp_SecureHashType")
                {
                    vnp.AddResponseData(key, value.ToString());
                }
            }

            var vnpHashSecret = _config["VNPay:HashSecret"];
            var vnpSecureHash = queryParams["vnp_SecureHash"].ToString();

            var isValid = vnp.ValidateSignature(vnpSecureHash, vnpHashSecret);

            if (!isValid)
                return ApiResponse<PaymentResponse>.Fail("Invalid VNPay signature");

            // ===== GET ORDER ID =====

            if (!int.TryParse(queryParams["vnp_TxnRef"], out int orderId))
                return ApiResponse<PaymentResponse>.Fail("Invalid order id");

            var payment = await _paymentRepo.GetByOrderIdAsync(orderId);

            if (payment == null)
                return ApiResponse<PaymentResponse>.Fail("Payment not found");

            // ===== CHECK AMOUNT =====

            if (!long.TryParse(queryParams["vnp_Amount"], out long vnpAmountRaw))
                return ApiResponse<PaymentResponse>.Fail("Invalid amount");

            decimal vnpAmount = vnpAmountRaw / 100m;

            if (vnpAmount != payment.Amount)
                return ApiResponse<PaymentResponse>.Fail("Invalid payment amount");

            // ===== RESPONSE CODE =====

            var responseCode = queryParams["vnp_ResponseCode"].ToString();

            switch (responseCode)
            {
                case "00":

                    payment.Status = "Success";

                    var order = await _orderRepo.GetByIdAsync(orderId);

                    if (order != null)
                    {
                        order.Status = "Confirmed";
                        order.UpdatedAt = DateTime.UtcNow;

                        await _orderRepo.UpdateAsync(order);
                    }

                    break;

                case "24":

                    payment.Status = "Canceled";
                    break;

                default:

                    payment.Status = "Failed";
                    break;
            }

            await _paymentRepo.UpdateAsync(payment);

            // ==============================
            // SAVE PAYMENT TRANSACTION
            // ==============================

            var transaction = new PaymentTransaction
            {
                PaymentId = payment.PaymentId,
                Gateway = "VNPay",
                GatewayTransactionId = queryParams["vnp_TransactionNo"].ToString(),
                Amount = vnpAmount,
                ResponseCode = responseCode,
                Status = payment.Status,
                RawData = System.Text.Json.JsonSerializer.Serialize(queryParams.ToDictionary(x => x.Key, x => x.Value.ToString())),
                CreatedAt = DateTime.UtcNow
            };

            await _paymentTransactionRepo.CreateAsync(transaction);

            // ==============================

            return ApiResponse<PaymentResponse>.Ok(new PaymentResponse
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                Status = payment.Status
            });
        }
        catch (Exception ex)
        {
            return ApiResponse<PaymentResponse>.Fail(ex.Message);
        }
    }
}