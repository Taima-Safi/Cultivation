namespace Cultivation.Database.Model;

public class InsecticideMixDetailModel : BaseModel
{
    public double Liter { get; set; }
    public double? Quantity { get; set; } //kg

    public long InsecticideId { get; set; }
    public InsecticideModel Insecticide { get; set; }

    public long InsecticideMixId { get; set; }
    public InsecticideMixModel InsecticideMix { get; set; }
}
