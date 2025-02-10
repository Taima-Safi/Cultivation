
using Cultivation.Database.Model;
using Cultivation.Dto.User;

namespace Cultivation.Repository.User;

public interface IUserRepo
{
    Task<long> AddAsync(UserFormDto dto);
    Task AddUserRolesAsync(IEnumerable<long> roleIds, long userId);
    Task<List<RoleModel>> GetUserRoleAsync(long userId);
    Task<UserDto> LoginAsync(LoginDto dto);
    Task RemoveAsync(long id);
    Task UpdateAsync(UserFormDto dto, long id);
}
