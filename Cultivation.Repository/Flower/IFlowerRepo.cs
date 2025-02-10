
using Cultivation.Database.Model;
using Cultivation.Dto.Flower;
using Cultivation.Dto.Common;

namespace Cultivation.Repository.Flower;

public interface IFlowerRepo
{
    Task AddAsync(AddFlowerFormDto dto, long cuttingLandId);
    Task AddExternalFlower(long flowerStoreId, int count);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<FlowerDto>>> GetAllAsync(DateTime? from, DateTime? to, long? cuttingLandId, string cuttingTitle
        , string cuttingColorCode, string worker, double? Long, int pageSize, int pageNum);
    Task<CommonResponseDto<List<FlowerStoreDto>>> GetAllFlowerStoreAsync(string code, int pageSize, int pageNum);
    Task<FlowerDto> GetByIdAsync(long id);
    Task<double> GetFlowerAverageInDonumAsync();
    Task<List<FlowerStoreModel>> GetFlowerStoreModelsByCodesAsync(List<string> cods);
    Task<FlowerModel> GetModelByIdAsync(long id);
    Task<List<FlowerModel>> GetModelsByIdsAsync(List<long> ids);
    Task<List<FlowerStoreModel>> GetStoreModelsByIdsAsync(List<long> ids);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, string note, string worker, double Long, int count, DateTime date);
}
