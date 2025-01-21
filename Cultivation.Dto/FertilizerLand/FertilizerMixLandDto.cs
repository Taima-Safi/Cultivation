using Cultivation.Dto.Fertilizer;
using Cultivation.Dto.Land;

namespace Cultivation.Dto.FertilizerLand;

public class FertilizerMixLandDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }

    public LandDto Land { get; set; }
    public GetFertilizerMixDto FertilizerMix { get; set; }
}
