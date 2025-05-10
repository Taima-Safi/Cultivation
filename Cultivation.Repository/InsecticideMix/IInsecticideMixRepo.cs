
using Cultivation.Dto.Insecticide;
using Cultivation.Shared.Enum;
using Cultivation.Dto.Common;

namespace Cultivation.Repository.InsecticideMix;

public interface IInsecticideMixRepo
{
    Task AddAsync(InsecticideMixFormDto dto);
    Task<CommonResponseDto<List<GetInsecticideMixDto>>> GetAllAsync(string title, string note, int pageSize, int pageNum);
    Task<List<InsecticideApplicableMixDto>> GetAllInsecticideApplicableMixAsync();
    Task<GetInsecticideMixDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, string title, string note, ColorType color);
}
