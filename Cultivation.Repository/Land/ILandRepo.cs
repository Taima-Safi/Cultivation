﻿using Cultivation.Database.Model;
using Cultivation.Dto.Land;

namespace Cultivation.Repository.Land;

public interface ILandRepo
{
    Task<long> AddAsync(LandFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<bool> CheckIfExistByIdsAsync(List<long> ids);
    Task<List<LandDto>> GetAllAsync(string title, double? size, bool justChildren);
    Task<LandDto> GetByIdAsync(long id);
    Task<LandModel> GetLandModelAsync(long id);
    Task RemoveAllAsync();
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, LandFormDto dto);
    //Task<List<LandDto>> GetLandRecursion();
}
