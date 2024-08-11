using Cultivation.Dto.Land;

namespace Cultivation.Repository.Land;

public interface ILandRepo
{
    Task<long> AddAsync(LandFormDto dto);
    Task<List<LandDto>> GetLandRecursion();
}
