using Cultivation.Shared.Enum;

namespace Cultivation.Dto.FertilizerLand;

public class FertilizerLandFormDto
{
    public DateTime Date { get; set; }
    public FertilizerType Type { get; set; }

    public List<long> CuttingLandIds { get; set; } = [];
    public List<FertilizerMixDto> Mixes { get; set; } = [];
}
