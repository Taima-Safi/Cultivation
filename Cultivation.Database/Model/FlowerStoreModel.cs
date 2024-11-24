namespace Cultivation.Database.Model;

public class FlowerStoreModel : BaseModel
{
    public int Count { get; set; }
    public string Code { get; set; }
    public int RemainedCount { get; set; }
    public double FlowerLong { get; set; }
    public ICollection<FlowerOrderModel> FlowerOrders { get; set; }

}
