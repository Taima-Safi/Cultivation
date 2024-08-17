
using Cultivation.Dto.Color;

namespace Cultivation.Repository.Color;

public interface IColorRepo
{
    Task<long> AddAsync(string title, string code);
    Task<List<ColorDto>> GetAllAsync(string title, string code);
    Task<ColorDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, string title, string code);
}
