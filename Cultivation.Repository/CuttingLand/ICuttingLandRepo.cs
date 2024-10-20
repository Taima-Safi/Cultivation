using Cultivation.Dto.CuttingLand;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.CuttingLand;

public interface ICuttingLandRepo
{
    Task<long> AddAsync(CuttingLandFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<CuttingLandDto>>> GetAllAsync(DateTime? date, int pageSize = 10, int pageNum = 0);
    Task<CuttingLandDto> GetByIdAsync(long id);
    Task UpdateIsActiveAsync(long id, bool isActive);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, DateTime date, long quantity);
}
