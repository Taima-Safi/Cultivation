using Cultivation.Dto.Common;
using Cultivation.Dto.InsecticideLand;
using Cultivation.Dto.Land;
using Microsoft.AspNetCore.Http;

namespace Cultivation.Repository.InsecticideLand;

public interface IInsecticideLandRepo
{
    Task AddAsync(InsecticideLandFormDto dto);
    Task AddMixLandAsync(long mixId, long landId);
    Task AddMixLandsAsync(long mixId, List<long> landIds);
    Task<bool> CheckIfExistAsync(long id);
    Task<(FormFile file, MemoryStream stream)> ExportExcelAsync(long landId, DateTime? from, DateTime? to, string fileName);
    Task<CommonResponseDto<List<GroupedInsecticideLandDto>>> GetAllAsync(/*DateTime? date, */string note, double? liter, double? quantity, DateTime? from, DateTime? to
        , long? landId, long? insecticideId, int pageSize, int pageNum);
    Task<InsecticideLandDto> GetByIdAsync(long id);
    Task<List<LandDto>> GetLandsWhichNotUsedInDayAsync(DateTime? date);
    Task<List<LandDto>> GetMixLandsAsync(string landTitle, string mixTitle, DateTime? mixedDate);
    Task RemoveAsync(long id);
    Task RemoveMixLandAsync(long mixLandId);
    Task UpdateAsync(long id, UpdateInsecticideLandFormDto dto);
}
