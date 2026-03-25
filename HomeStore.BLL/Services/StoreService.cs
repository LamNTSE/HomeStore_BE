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

    public async Task<ApiResponse<List<StoreLocationDto>>> GetAllStoresAsync()
    {
        var stores = await _storeRepo.GetAllAsync();
        var dtos = _mapper.Map<List<StoreLocationDto>>(stores);

        dtos = dtos.OrderBy(d => d.LocationId).ToList();

        return ApiResponse<List<StoreLocationDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<StoreLocationDto>> GetStoreByIdAsync(int locationId)
    {
        var store = await _storeRepo.GetByIdAsync(locationId);
        if (store == null) return ApiResponse<StoreLocationDto>.Fail("Store not found.");
        return ApiResponse<StoreLocationDto>.Ok(_mapper.Map<StoreLocationDto>(store));
    }

    public async Task<ApiResponse<StoreLocationDto>> CreateStoreAsync(StoreLocationDto dto)
    {
        var entity = new Domain.Entities.StoreLocation
        {
            StoreName = dto.StoreName,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Phone = dto.Phone,
            IsActive = true
        };
        var created = await _storeRepo.CreateAsync(entity);
        return ApiResponse<StoreLocationDto>.Ok(_mapper.Map<StoreLocationDto>(created));
    }

    public async Task<ApiResponse<StoreLocationDto>> UpdateStoreAsync(int locationId, StoreLocationDto dto)
    {
        var existing = await _storeRepo.GetByIdAsync(locationId);
        if (existing == null) return ApiResponse<StoreLocationDto>.Fail("Store not found.");

        existing.StoreName = dto.StoreName;
        existing.Address = dto.Address;
        existing.Latitude = dto.Latitude;
        existing.Longitude = dto.Longitude;
        existing.Phone = dto.Phone;

        await _storeRepo.UpdateAsync(existing);
        return ApiResponse<StoreLocationDto>.Ok(_mapper.Map<StoreLocationDto>(existing));
    }

    //private static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
    //{
    //    const double R = 6371; // Earth radius in km
    //    var dLat = ToRad(lat2 - lat1);
    //    var dLng = ToRad(lng2 - lng1);
    //    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
    //            Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
    //            Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
    //    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    //    return Math.Round(R * c, 2);
    //}

    //private static double ToRad(double deg) => deg * Math.PI / 180;
}
