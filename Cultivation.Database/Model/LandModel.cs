namespace Cultivation.Database.Model;

public class LandModel : BaseModel
{
    public string Title { get; set; }
    public double Size { get; set; }
    public string Location { get; set; }

    public long? ParentId { get; set; }
    public LandModel Parent { get; set; }
    public ICollection<LandModel> Children { get; set; }
    public ICollection<CuttingLandModel> CuttingLands { get; set; }
    public ICollection<FertilizerLandModel> FertilizerLands { get; set; }
    public ICollection<InsecticideLandModel> InsecticideLands { get; set; }
    public ICollection<FertilizerMixLandModel> FertilizerMixLands { get; set; }
    public ICollection<InsecticideMixLandModel> InsecticideMixLands { get; set; }

}
