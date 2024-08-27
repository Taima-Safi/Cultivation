using Cultivation.Dto.Fertilizer;

namespace Cultivation.Repository.Fertilizer;

public interface IFertilizerRepo
{
    Task<long> AddAsync(FertilizerFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<bool> CheckIfExistByIdsAsync(List<long> ids);
    Task<List<FertilizerDto>> GetAllAsync(string npk, string title, string publicTitle, string description);
    Task<FertilizerDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, FertilizerFormDto dto);
}
