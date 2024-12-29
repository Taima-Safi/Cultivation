namespace Cultivation.Database.Model;

public class FertilizerMixLandModel : BaseModel
{
    public DateTime Date { get; set; }

    public long CuttingLandId { get; set; }
    public CuttingLandModel CuttingLand { get; set; }

    public long FertilizerMixId { get; set; }
    public FertilizerMixModel FertilizerMix { get; set; }
}
