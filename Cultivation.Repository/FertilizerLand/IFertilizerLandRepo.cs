﻿
using Cultivation.Dto.FertilizerLand;
using Cultivation.Dto.Land;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.FertilizerLand;

public interface IFertilizerLandRepo
{
    Task AddAsync(FertilizerLandFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<GroupedFertilizerLandDto>>> GetAllAsync(DateTime? date, int pageSize, int pageNum);
    Task<FertilizerLandDto> GetByIdAsync(long id);
    Task<CommonResponseDto<List<FertilizerLandDto>>> GetFertilizersLandAsync(long landId, DateTime? from, DateTime? to, int pageSize, int pageNum);
    Task<List<LandDto>> GetLandsWhichNotUsedInDayAsync(DateTime? date);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, UpdateFertilizerLandDto dto);
}
