﻿using Cultivation.Dto.CuttingLand;

namespace Cultivation.Repository.CuttingLand;

public interface ICuttingLandRepo
{
    Task<long> AddAsync(CuttingLandFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<List<CuttingLandDto>> GetAllAsync(DateTime? date);
    Task<CuttingLandDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, DateTime date, long quantity);
}