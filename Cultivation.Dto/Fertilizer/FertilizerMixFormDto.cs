using Cultivation.Dto.FertilizerLand;
using Cultivation.Shared.Enum;

namespace Cultivation.Dto.Fertilizer;

public class FertilizerMixFormDto
{
    public string Title { get; set; }
    public ColorType Color { get; set; }
    public FertilizerType Type { get; set; }

    public List<FertilizerMixDto> Mixes { get; set; } = [];
}
