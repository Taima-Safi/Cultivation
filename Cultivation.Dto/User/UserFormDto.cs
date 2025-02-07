using Cultivation.Shared.Enum;

namespace Cultivation.Dto.User;

public class UserFormDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public UserType Type { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public List<long> RoleIdsToAdd { get; set; } = [];
    public List<long> RoleIdsToRemove { get; set; } = [];
}
