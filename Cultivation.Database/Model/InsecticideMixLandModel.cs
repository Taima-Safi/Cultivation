namespace Cultivation.Database.Model;

public class InsecticideMixLandModel : BaseModel
{
    public DateTime Date { get; set; }

    public long LandId { get; set; }
    public LandModel Land { get; set; }

    public long InsecticideMixId { get; set; }
    public InsecticideMixModel InsecticideMix { get; set; }
}
