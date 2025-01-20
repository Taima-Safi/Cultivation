
using Cultivation.Database.Model;
using Cultivation.Dto.FertilizerLand;
using Cultivation.Dto.Land;
using FourthPro.Dto.Common;
using Microsoft.AspNetCore.Http;


namespace Cultivation.Repository.FertilizerLand;

public interface IFertilizerLandRepo
{
    Task AddAsync(FertilizerLandFormDto dto);
    Task AddMixLandAsync(long mixId, long cuttingLandId);
    Task AddMixLandsAsync(long mixId, List<long> cuttingLandIds);
    Task<bool> CheckIfExistAsync(long id);
    Task<(FormFile file, MemoryStream stream)> ExportExcelAsync(long landId, DateTime? from, DateTime? to, string fileName);
    Task<CommonResponseDto<List<GroupedFertilizerLandDto>>> GetAllAsync(long? landId, DateTime? from, DateTime? to, int pageSize, int pageNum);
    Task<FertilizerLandDto> GetByIdAsync(long id);
    Task<CommonResponseDto<List<FertilizerLandModel>>> GetFertilizersLandAsync(long landId, DateTime? from, DateTime? to);
    Task<List<LandDto>> GetLandsWhichNotUsedInDayAsync(DateTime? date);
    Task<List<LandDto>> GetMixLandsAsync(string landTitle, string mixTitle, DateTime? mixedDate);
    Task RemoveAsync(long id);
    Task RemoveMixLandAsync(long mixLandId);
    Task RemoveMixLandsAsync(List<long> mixLandIds);
    Task UpdateAsync(long id, UpdateFertilizerLandDto dto);
}
