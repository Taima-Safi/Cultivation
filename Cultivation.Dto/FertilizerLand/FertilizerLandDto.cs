using Cultivation.Dto.Fertilizer;
using Cultivation.Dto.Land;
using Cultivation.Shared.Enum;

namespace Cultivation.Dto.FertilizerLand;

public class FertilizerLandDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public double Quantity { get; set; } //kg
    public FertilizerType Type { get; set; }

    public LandDto Land { get; set; }
    public FertilizerDto Fertilizer { get; set; }
}
