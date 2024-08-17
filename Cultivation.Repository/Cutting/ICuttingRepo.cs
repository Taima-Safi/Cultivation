
using Cultivation.Dto.Cutting;

namespace Cultivation.Repository.Cutting;

public interface ICuttingRepo
{
    Task<long> AddAsync(string title, string type, int age);
    Task<long> AddCuttingColorAsync(CuttingColorFormDto dto);
    Task<List<CuttingDto>> GetAllAsync(string title, string type, int? age);
    Task<List<CuttingColorDto>> GetAllCuttingColorAsync(string code);
    Task<CuttingDto> GetByIdAsync(long id);
    Task<CuttingColorDto> GetCuttingColorByIdAsync(long id);
    Task RemoveAsync(long id);
    Task RemoveCuttingColorAsync(long id);
    Task UpdateAsync(long id, string title, string type, int age);
    Task UpdateCuttingColorAsync(long id, CuttingColorFormDto dto);
}
