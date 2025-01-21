namespace Cultivation.Database.Model;

public class FertilizerMixLandModel : BaseModel
{
    public DateTime Date { get; set; }

    public long LandId { get; set; }
    public LandModel Land { get; set; }

    public long FertilizerMixId { get; set; }
    public FertilizerMixModel FertilizerMix { get; set; }
}
