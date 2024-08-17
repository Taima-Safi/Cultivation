using Cultivation.Dto.Fertilizer;
using Cultivation.Shared.Enum;

namespace Cultivation.Repository.Fertilizer;

public interface IFertilizerRepo
{
    Task<long> AddAsync(FertilizerFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<List<FertilizerDto>> GetAllAsync(string npk, string title, string publicTitle, string description, FertilizerType? type);
    Task<FertilizerDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, FertilizerFormDto dto);
}
