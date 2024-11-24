namespace Cultivation.Database.Model;

public class FlowerOrderModel : BaseModel
{
    public int Count { get; set; }

    public long OrderId { get; set; }
    public OrderModel Order { get; set; }
    public long FlowerStoreId { get; set; }
    public FlowerStoreModel FlowerStore { get; set; }
}