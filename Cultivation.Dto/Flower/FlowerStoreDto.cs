namespace Cultivation.Dto.Flower;

public class FlowerStoreDto
{
    public long Id { get; set; }
    public int Count { get; set; } // before send order
    public string Code { get; set; }
    public int TotalCount { get; set; } // const
    public int RemainedCount { get; set; }// after send order
    public double FlowerLong { get; set; }
    public int TrashedCount { get; set; }
    public int ExternalCount { get; set; }
    //public List<OrderDetailModel> OrderDetails { get; set; } =[];
}
