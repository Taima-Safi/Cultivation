
using Cultivation.Dto.Flower;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.Flower;

public interface IFlowerRepo
{
    Task<long> AddAsync(int count, string note, DateTime date, long cuttingLandId);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<FlowerDto>>> GetAllAsync(DateTime? from, DateTime? to, long? cuttingLandId, string cuttingTitle
        , string cuttingColorCode, string colorTitle, int pageSize, int pageNum);
    Task<FlowerDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, string note, int count, DateTime date);
}
