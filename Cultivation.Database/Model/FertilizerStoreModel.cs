namespace Cultivation.Database.Model;

public class FertilizerStoreModel : BaseModel
{
    public double TotalQuantity { get; set; }

    public long FertilizerId { get; set; }
    public FertilizerModel Fertilizer { get; set; }
}