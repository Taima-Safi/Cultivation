namespace Cultivation.Database.Model;

public class InsecticideMixLandModel : BaseModel
{
    public DateTime Date { get; set; }

    public long CuttingLandId { get; set; }
    public CuttingLandModel CuttingLand { get; set; }

    public long InsecticideMixId { get; set; }
    public InsecticideMixModel InsecticideMix { get; set; }
}
