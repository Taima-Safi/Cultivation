namespace Cultivation.Database.Model;

public class UserRoleModel : BaseModel
{
    public long UserId { get; set; }
    public UserModel User { get; set; }
    public long RoleId { get; set; }
    public RoleModel Role { get; set; }
}