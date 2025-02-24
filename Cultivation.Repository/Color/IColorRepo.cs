
using Cultivation.Dto.Color;
using Cultivation.Dto.Common;

namespace Cultivation.Repository.Color;

public interface IColorRepo
{
    Task<long> AddAsync(string title, string code);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<ColorDto>>> GetAllAsync(string title, string code, int pageSize, int pageNum);
    Task<ColorDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, string title, string code);
}
