using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.FertilizerLand;

namespace Cultivation.Dto.Land;

public class LandDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public double Size { get; set; }
    public string Location { get; set; }

    public long? ParentId { get; set; }
    public List<LandDto> Children { get; set; } = [];
    public List<CuttingLandDto> CuttingLands { get; set; } = [];
    public List<FertilizerMixLandDto> FertilizerMixLands { get; set; } = [];
}
