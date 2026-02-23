using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Store;

namespace HomeStore.Domain.Interfaces.Services;

public interface IStoreService
{
    Task<ApiResponse<List<StoreLocationDto>>> GetAllStoresAsync(double? userLat = null, double? userLng = null);
    Task<ApiResponse<StoreLocationDto>> GetStoreByIdAsync(int locationId);
}
