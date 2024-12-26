
using Cultivation.Dto.Insecticide;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.InsecticideMix;

public interface IInsecticideMixRepo
{
    Task AddAsync(InsecticideMixFormDto dto);
    Task<CommonResponseDto<List<GetInsecticideMixDto>>> GetAllAsync(string title, string note, int pageSize, int pageNum);
    Task<GetInsecticideMixDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, string title, string note);
}
