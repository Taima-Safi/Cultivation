namespace Cultivation.Database.Model;

public class FlowerStoreModel : BaseModel
{
    public int Count { get; set; } // before send order
    public string Code { get; set; }
    public int TotalCount { get; set; } // const
    public int RemainedCount { get; set; }// after send order
    public double FlowerLong { get; set; }
    public ICollection<OrderDetailModel> OrderDetails { get; set; }

}
