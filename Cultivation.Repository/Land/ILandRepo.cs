using Cultivation.Dto.Land;

namespace Cultivation.Repository.Land;

public interface ILandRepo
{
    Task<long> AddAsync(LandFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<List<LandDto>> GetAllAsync(string title, double? size);
    Task<LandDto> GetByIdAsync(long id);
    //Task<List<LandDto>> GetLandRecursion();
}
