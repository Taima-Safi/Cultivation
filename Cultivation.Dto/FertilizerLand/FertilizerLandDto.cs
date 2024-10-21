using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Fertilizer;
using Cultivation.Shared.Enum;

namespace Cultivation.Dto.FertilizerLand;

public class FertilizerLandDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public double Quantity { get; set; } //kg
    public FertilizerType Type { get; set; }

    public CuttingLandDto CuttingLand { get; set; }
    public FertilizerDto Fertilizer { get; set; }
}
