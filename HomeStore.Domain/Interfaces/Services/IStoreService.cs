using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Store;

namespace HomeStore.Domain.Interfaces.Services;

public interface IStoreService
{
    Task<ApiResponse<List<StoreLocationDto>>> GetAllStoresAsync();
    Task<ApiResponse<StoreLocationDto>> GetStoreByIdAsync(int locationId);
    Task<ApiResponse<StoreLocationDto>> CreateStoreAsync(StoreLocationDto dto);
    Task<ApiResponse<StoreLocationDto>> UpdateStoreAsync(int locationId, StoreLocationDto dto);
}
