using Cultivation.Shared.Enum;

namespace Cultivation.Dto.Fertilizer;

public class GetFertilizerMixDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public ColorType Color { get; set; }
    public FertilizerType Type { get; set; }

    public List<FertilizerMixDetailDto> MixDetails { get; set; } = [];
}
