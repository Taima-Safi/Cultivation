using Cultivation.Dto.Token;
using Cultivation.Shared.Enum;

namespace Cultivation.Dto.User;

public class UserDto
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => FirstName + " " + LastName;
    public string Email { get; set; }
    public UserType Type { get; set; }
    public string PhoneNumber { get; set; }
    public string HashPassword { get; set; }

    //public long RoleId { get; set; }
    //public RoleModel Role { get; set; }
    public TokenDto Token { get; set; }
    public List<UserRoleDto> Roles { get; set; } = [];
}
