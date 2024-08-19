﻿
using Cultivation.Dto.FertilizerLand;
using Cultivation.Shared.Enum;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.FertilizerLand;

public interface IFertilizerLandRepo
{
    Task AddAsync(FertilizerLandFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<FertilizerLandDto>>> GetAllAsync(int pageSize, int pageNum);
    Task<FertilizerLandDto> GetByIdAsync(long id);
    Task<CommonResponseDto<List<FertilizerLandDto>>> GetFertilizersLandAsync(long landId, DateTime? from, DateTime? to, int pageSize, int pageNum);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, double? quantity, DateTime? date, FertilizerType? type, long? landId, long? fertilizerId);
}
