using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Fertilizer;

namespace Cultivation.Dto.FertilizerLand;

public class FertilizerMixLandDto
{
    public DateTime Date { get; set; }

    public CuttingLandDto CuttingLand { get; set; }
    public GetFertilizerMixDto FertilizerMix { get; set; }
}
