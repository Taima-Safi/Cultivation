
using Cultivation.Dto.Flower;

namespace Cultivation.Repository.Flower;

public interface IFlowerRepo
{
    Task<long> AddAsync(int count, DateTime date, long cuttingLandId);
    Task<bool> CheckIfExistAsync(long id);
    Task<List<FlowerDto>> GetAllAsync(DateTime? date, long? cuttingLandId);
    Task<FlowerDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, int count, DateTime date);
}
