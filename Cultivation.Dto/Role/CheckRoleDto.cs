namespace Cultivation.Dto.Role;

public class CheckRoleDto
{
    //public long Id { get; set; }
    //public string Title { get; set; }
    public bool FullAccess { get; set; }
    public bool DepoAccess { get; set; }
    public bool OrderAccess { get; set; }
    public bool CuttingLandAccess { get; set; }
}
