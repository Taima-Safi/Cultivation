using Cultivation.Dto.Fertilizer;
using Cultivation.Shared.Enum;
using Cultivation.Dto.Common;

namespace Cultivation.Repository.FertilizerMix;

public interface IFertilizerMixRepo
{
    Task AddAsync(FertilizerMixFormDto dto);
    Task<CommonResponseDto<List<GetFertilizerMixDto>>> GetAllAsync(string title, int pageSize, int pageNum);
    Task<GetFertilizerMixDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, string title, FertilizerType type, ColorType color);
}
