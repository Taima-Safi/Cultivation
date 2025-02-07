namespace Cultivation.Database.Model;

public class RoleModel : BaseModel
{
    public string Title { get; set; }
    public ICollection<UserRoleModel> UserRoles { get; set; }
}
