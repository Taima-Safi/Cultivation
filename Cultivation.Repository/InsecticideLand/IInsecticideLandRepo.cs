using Cultivation.Dto.InsecticideLand;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.InsecticideLand;

public interface IInsecticideLandRepo
{
    Task<long> AddAsync(InsecticideLandFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<InsecticideLandDto>>> GetAllAsync(/*DateTime? date, */string note, double? liter, double? quantity, DateTime? from, DateTime? to
        , long? landId, long? insecticideId, int pageSize, int pageNum);
    Task<InsecticideLandDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, InsecticideLandFormDto dto);
}
