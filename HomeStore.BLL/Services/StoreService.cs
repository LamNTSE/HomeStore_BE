using AutoMapper;
using HomeStore.Domain.DTOs.Common;
using HomeStore.Domain.DTOs.Store;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;

namespace HomeStore.BLL.Services;

public class StoreService : IStoreService
{
    private readonly IStoreLocationRepository _storeRepo;
    private readonly IMapper _mapper;

    public StoreService(IStoreLocationRepository storeRepo, IMapper mapper)
    {
        _storeRepo = storeRepo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<StoreLocationDto>>> GetAllStoresAsync(double? userLat = null, double? userLng = null)
    {
        var stores = await _storeRepo.GetAllAsync();
        var dtos = _mapper.Map<List<StoreLocationDto>>(stores);

        if (userLat.HasValue && userLng.HasValue)
        {
            foreach (var dto in dtos)
            {
                dto.DistanceKm = CalculateDistance(userLat.Value, userLng.Value, dto.Latitude, dto.Longitude);
            }
            dtos = dtos.OrderBy(d => d.DistanceKm).ToList();
        }

        return ApiResponse<List<StoreLocationDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<StoreLocationDto>> GetStoreByIdAsync(int locationId)
    {
        var store = await _storeRepo.GetByIdAsync(locationId);
        if (store == null) return ApiResponse<StoreLocationDto>.Fail("Store not found.");
        return ApiResponse<StoreLocationDto>.Ok(_mapper.Map<StoreLocationDto>(store));
    }

    private static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
    {
        const double R = 6371; // Earth radius in km
        var dLat = ToRad(lat2 - lat1);
        var dLng = ToRad(lng2 - lng1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Math.Round(R * c, 2);
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
