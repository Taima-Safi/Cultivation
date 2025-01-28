using Cultivation.Shared.Enum;

namespace Cultivation.Database.Model;

public class FertilizerLandModel : BaseModel
{
    public DateTime Date { get; set; }
    public double Quantity { get; set; } //kg
    public FertilizerType Type { get; set; }

    public long LandId { get; set; }
    public LandModel Land { get; set; }
    public long FertilizerId { get; set; }
    public FertilizerModel Fertilizer { get; set; }
}