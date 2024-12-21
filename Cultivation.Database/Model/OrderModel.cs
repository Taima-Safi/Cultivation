namespace Cultivation.Database.Model;

public class OrderModel : BaseModel
{
    public string Number { get; set; }
    public bool IsBought { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? BoughtDate { get; set; }

    public long ClientId { get; set; }
    public ClientModel Client { get; set; }
    public ICollection<OrderDetailModel> OrderDetails { get; set; }
}