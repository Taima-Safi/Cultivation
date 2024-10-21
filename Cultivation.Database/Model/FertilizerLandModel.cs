using Cultivation.Shared.Enum;

namespace Cultivation.Database.Model;

public class FertilizerLandModel : BaseModel
{
    public DateTime Date { get; set; }
    public double Quantity { get; set; } //kg
    public FertilizerType Type { get; set; }

    public long CuttingLandId { get; set; }
    public CuttingLandModel CuttingLand { get; set; }
    public long FertilizerId { get; set; }
    public FertilizerModel Fertilizer { get; set; }
}