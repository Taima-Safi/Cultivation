
using Cultivation.Dto.Flower;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.Flower;

public interface IFlowerRepo
{
    Task AddAsync(AddFlowerFormDto dto, long cuttingLandId);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<FlowerDto>>> GetAllAsync(DateTime? from, DateTime? to, long? cuttingLandId, string cuttingTitle
        , string cuttingColorCode, string colorTitle, double? Long, int pageSize, int pageNum);
    Task<FlowerDto> GetByIdAsync(long id);
    Task<double> GetFlowerAverageInDonumAsync();
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, string note, double Long, int count, DateTime date);
}
