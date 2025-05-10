using Cultivation.Dto.Common;
using Cultivation.Dto.Insecticide;
using Cultivation.Shared.Enum;

namespace Cultivation.Repository.Insecticide;

public interface IInsecticideRepo
{
    Task<long> AddAsync(InsecticideFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<bool> CheckIfExistByIdsAsync(List<long> ids);
    Task<CommonResponseDto<List<InsecticideDto>>> GetAllAsync(string title, string publicTitle, string description, InsecticideType? type, int pageSize, int pageNum);
    Task<CommonResponseDto<List<InsecticideStoreDto>>> GetAllInsecticideStoreAsync(string insecticideTitle, InsecticideType? type, DateTime? date, int pageSize, int pageNum);
    Task<CommonResponseDto<List<InsecticideTransactionDto>>> GetAllInsecticideTransactionAsync(string insecticideTitle, DateTime? from, DateTime? to, int pageSize, int pageNum);
    Task<InsecticideDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, InsecticideFormDto dto);
    Task UpdateStoreAsync(long insecticideId, double quantity, DateTime date, bool isAdd);
    Task UpdateStoreForMixAsync(long mixId, double donumNum, DateTime date);
}
