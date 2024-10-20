using Cultivation.Dto.InsecticideLand;
using Cultivation.Dto.Land;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.InsecticideLand;

public interface IInsecticideLandRepo
{
    Task AddAsync(InsecticideLandFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<GroupedInsecticideLandDto>>> GetAllAsync(/*DateTime? date, */string note, double? liter, double? quantity, DateTime? from, DateTime? to
        , long? landId, long? insecticideId, int pageSize, int pageNum);
    Task<InsecticideLandDto> GetByIdAsync(long id);
    Task<List<LandDto>> GetLandsWhichNotUsedInDayAsync(DateTime? date);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, UpdateInsecticideLandFormDto dto);
}
