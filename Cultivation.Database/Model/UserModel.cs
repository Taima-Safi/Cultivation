using Cultivation.Shared.Enum;

namespace Cultivation.Database.Model;

public class UserModel : BaseModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => FirstName + " " + LastName;
    public string Email { get; set; }
    public UserType Type { get; set; }
    public string PhoneNumber { get; set; }
    public string HashPassword { get; set; }

    //public long RoleId { get; set; }
    //public RoleModel Role { get; set; }
    public ICollection<TokenModel> Tokens { get; set; }
    public ICollection<UserRoleModel> UserRoles { get; set; }
}
