using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Vouchers;

namespace HomeStore.Domain.Interfaces.Services;

public interface IVoucherService
{
    Task<ApiResponse<List<VoucherDto>>> GetAllVouchersAsync();
    // thêm mới cho Customer
    Task<ApiResponse<List<VoucherDto>>> GetAvailableVouchersAsync();
    Task<ApiResponse<VoucherDto>> GetVoucherByIdAsync(int voucherId);
    Task<ApiResponse<VoucherDto>> GetVoucherByCodeAsync(string code);
    Task<ApiResponse<VoucherDto>> CreateVoucherAsync(CreateVoucherRequest request);
    Task<ApiResponse<VoucherDto>> UpdateVoucherAsync(int voucherId, UpdateVoucherRequest request);
    Task<ApiResponse<bool>> DeleteVoucherAsync(int voucherId);
}
