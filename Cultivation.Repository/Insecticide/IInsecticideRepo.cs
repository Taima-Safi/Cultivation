using Cultivation.Dto.Insecticide;
using Cultivation.Shared.Enum;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.Insecticide;

public interface IInsecticideRepo
{
    Task<long> AddAsync(InsecticideFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<InsecticideDto>>> GetAllAsync(string title, string publicTitle, string description, InsecticideType? type, int pageSize, int pageNum);
    Task<InsecticideDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, InsecticideFormDto dto);
}
