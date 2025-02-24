using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Common;

namespace Cultivation.Repository.CuttingLand;

public interface ICuttingLandRepo
{
    Task AddAsync(CuttingLandFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<CuttingLandDto>>> GetAllAsync(DateTime? date, int pageSize = 10, int pageNum = 0);
    Task<CuttingLandDto> GetByIdAsync(long id);
    Task UpdateIsActiveAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, DateTime date, long quantity);
    Task<List<long>> GetActiveCuttingLandIdsAsync(List<long> landId);
    Task<long> GetCuttingLandIdAsync(long landId);
    Task<bool> CheckIfExistByIdsAsync(List<long> ids);
}
