using AutoMapper;
using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Vouchers;
using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;

namespace HomeStore.BLL.Services;

public class VoucherService : IVoucherService
{
    private readonly IVoucherRepository _voucherRepo;
    private readonly IMapper _mapper;

    public VoucherService(IVoucherRepository voucherRepo, IMapper mapper)
    {
        _voucherRepo = voucherRepo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<VoucherDto>>> GetAllVouchersAsync()
    {
        var vouchers = await _voucherRepo.GetAllAsync();
        return ApiResponse<List<VoucherDto>>.Ok(_mapper.Map<List<VoucherDto>>(vouchers));
    }

    public async Task<ApiResponse<VoucherDto>> GetVoucherByIdAsync(int voucherId)
    {
        var voucher = await _voucherRepo.GetByIdAsync(voucherId);
        if (voucher == null) return ApiResponse<VoucherDto>.Fail("Voucher not found.");
        return ApiResponse<VoucherDto>.Ok(_mapper.Map<VoucherDto>(voucher));
    }

    public async Task<ApiResponse<VoucherDto>> GetVoucherByCodeAsync(string code)
    {
        var voucher = await _voucherRepo.GetByCodeAsync(code);
        if (voucher == null) return ApiResponse<VoucherDto>.Fail("Voucher not found.");
        return ApiResponse<VoucherDto>.Ok(_mapper.Map<VoucherDto>(voucher));
    }

    public async Task<ApiResponse<VoucherDto>> CreateVoucherAsync(CreateVoucherRequest request)
    {
        var existing = await _voucherRepo.GetByCodeAsync(request.Code);
        if (existing != null) return ApiResponse<VoucherDto>.Fail("Voucher code already exists.");

        var voucher = new Voucher
        {
            Code = request.Code.Trim().ToUpper(),
            DiscountType = request.DiscountType == "Fixed" ? "Fixed" : "Percent",
            DiscountValue = request.DiscountValue,
            MinOrderValue = request.MinOrderValue,
            MaxUsageCount = request.MaxUsageCount,
            StartDate = request.StartDate,
            ExpiryDate = request.ExpiryDate,
            IsActive = true
        };

        await _voucherRepo.CreateAsync(voucher);
        return ApiResponse<VoucherDto>.Ok(_mapper.Map<VoucherDto>(voucher), "Voucher created.");
    }

    public async Task<ApiResponse<VoucherDto>> UpdateVoucherAsync(int voucherId, UpdateVoucherRequest request)
    {
        var voucher = await _voucherRepo.GetByIdAsync(voucherId);
        if (voucher == null) return ApiResponse<VoucherDto>.Fail("Voucher not found.");

        if (request.Code != null)
        {
            var existing = await _voucherRepo.GetByCodeAsync(request.Code.Trim().ToUpper());
            if (existing != null && existing.VoucherId != voucherId)
                return ApiResponse<VoucherDto>.Fail("Voucher code already exists.");
            voucher.Code = request.Code.Trim().ToUpper();
        }
        if (request.DiscountType != null)
            voucher.DiscountType = request.DiscountType == "Fixed" ? "Fixed" : "Percent";
        if (request.DiscountValue.HasValue) voucher.DiscountValue = request.DiscountValue.Value;
        if (request.MinOrderValue.HasValue) voucher.MinOrderValue = request.MinOrderValue.Value;
        if (request.MaxUsageCount.HasValue) voucher.MaxUsageCount = request.MaxUsageCount.Value;
        if (request.StartDate.HasValue) voucher.StartDate = request.StartDate.Value;
        if (request.ExpiryDate.HasValue) voucher.ExpiryDate = request.ExpiryDate.Value;
        if (request.IsActive.HasValue) voucher.IsActive = request.IsActive.Value;

        await _voucherRepo.UpdateAsync(voucher);
        return ApiResponse<VoucherDto>.Ok(_mapper.Map<VoucherDto>(voucher), "Voucher updated.");
    }

    public async Task<ApiResponse<bool>> DeleteVoucherAsync(int voucherId)
    {
        var voucher = await _voucherRepo.GetByIdAsync(voucherId);
        if (voucher == null) return ApiResponse<bool>.Fail("Voucher not found.");
        await _voucherRepo.DeleteAsync(voucherId);
        return ApiResponse<bool>.Ok(true, "Voucher deleted.");
    }
}
