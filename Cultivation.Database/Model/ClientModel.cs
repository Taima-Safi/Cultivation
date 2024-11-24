namespace Cultivation.Database.Model;

public class ClientModel : BaseModel
{
    public string Name { get; set; }
    public bool IsLocal { get; set; }
    public string PhoneNumber { get; set; }
    public string CodePhoneNumber { get; set; }
    public ICollection<OrderModel> Orders { get; set; }
}
