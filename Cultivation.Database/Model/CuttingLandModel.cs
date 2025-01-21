namespace Cultivation.Database.Model;

public class CuttingLandModel : BaseModel
{
    public DateTime Date { get; set; }
    public long Quantity { get; set; }
    public bool IsActive { get; set; }

    public long LandId { get; set; }
    public LandModel Land { get; set; }

    public long CuttingColorId { get; set; }
    public CuttingColorModel CuttingColor { get; set; }

    public ICollection<FlowerModel> Flowers { get; set; }
    public ICollection<FertilizerLandModel> FertilizerLands { get; set; }
    public ICollection<InsecticideLandModel> InsecticideLands { get; set; }
    //public ICollection<FertilizerMixLandModel> FertilizerMixLands { get; set; }
    public ICollection<InsecticideMixLandModel> InsecticideMixLands { get; set; }
}
