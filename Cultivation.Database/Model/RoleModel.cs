namespace Cultivation.Database.Model;

public class RoleModel : BaseModel
{
    public string Title { get; set; }
    public bool FullAccess { get; set; }
    public bool DepoAccess { get; set; }
    public bool OrderAccess { get; set; }
    public bool CuttingLandAccess { get; set; }
    public ICollection<UserRoleModel> UserRoles { get; set; }
}
