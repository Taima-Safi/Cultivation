using Cultivation.Dto.Fertilizer;
using Cultivation.Dto.Common;

namespace Cultivation.Repository.Fertilizer;

public interface IFertilizerRepo
{
    Task<long> AddAsync(FertilizerFormDto dto);
    Task AddToStoreAsync(long fertilizerId, double quantity, DateTime date, bool isAdd);
    Task<bool> CheckIfExistAsync(long id);
    Task<bool> CheckIfExistByIdsAsync(List<long> ids);
    Task<CommonResponseDto<List<FertilizerDto>>> GetAllAsync(string npk, string title, string publicTitle, string description, int pageSize, int pageNum);
    Task<FertilizerDto> GetByIdAsync(long id);
    Task<CommonResponseDto<List<FertilizerTransactionDto>>> GetAllFertilizerTransactionAsync(string fertilizerTitle, DateTime? from, DateTime? to, int pageSize, int pageNum);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, FertilizerFormDto dto);
}
