using Cultivation.Dto.Fertilizer;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.Fertilizer;

public interface IFertilizerRepo
{
    Task<long> AddAsync(FertilizerFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<bool> CheckIfExistByIdsAsync(List<long> ids);
    Task<CommonResponseDto<List<FertilizerDto>>> GetAllAsync(string npk, string title, string publicTitle, string description, int pageSize, int pageNum);
    Task<FertilizerDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, FertilizerFormDto dto);
}
