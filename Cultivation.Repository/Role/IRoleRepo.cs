
using Cultivation.Dto.Role;

namespace Cultivation.Repository.Role;

public interface IRoleRepo
{
    Task<long> AddAsync(RoleFormDto dto);
    Task<List<RoleDto>> GetAllAsync(string title);
    Task<RoleDto> GetByIdAsync(long id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, RoleFormDto dto);
}
