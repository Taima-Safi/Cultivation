namespace Cultivation.Database.Model;

public class InsecticideStoreModel : BaseModel
{
    public double TotalQuantity { get; set; }

    public long InsecticideId { get; set; }
    public InsecticideModel Insecticide { get; set; }
}
