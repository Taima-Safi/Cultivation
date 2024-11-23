using Cultivation.Shared.Enum;

namespace Cultivation.Dto.FertilizerLand;

public class UpdateFertilizerLandDto
{
    public DateTime Date { get; set; }
    public double Quantity { get; set; } //kg
    public FertilizerType Type { get; set; }

    public long CuttingLandId { get; set; }
    public long FertilizerId { get; set; }
}
