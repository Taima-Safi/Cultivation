namespace Cultivation.Database.Model;

public class InsecticideLandModel : BaseModel
{
    // public string File { get; set; }
    public string Note { get; set; }
    public double Liter { get; set; }
    public DateTime Date { get; set; }
    public double? Quantity { get; set; } // for one liter

    public long CuttingLandId { get; set; }
    public CuttingLandModel CuttingLand { get; set; }
    public long InsecticideId { get; set; }
    public InsecticideModel Insecticide { get; set; }
}
