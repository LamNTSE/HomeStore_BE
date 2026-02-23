using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Payments;
using Microsoft.AspNetCore.Http;

namespace HomeStore.Domain.Interfaces.Services;

public interface IPaymentService
{
    Task<ApiResponse<PaymentResponse>> CreatePaymentAsync(int userId, PaymentRequest request, HttpContext httpContext);
    Task<ApiResponse<PaymentResponse>> HandleVnpayReturnAsync(IQueryCollection queryParams);
}
