using Cultivation.Dto.Color;

namespace Cultivation.Dto.Cutting;

public class CuttingColorDto
{
    public long Id { get; set; }
    public string Code { get; set; }// flower color + type
    public ColorDto Color { get; set; }
    public CuttingDto Cutting { get; set; }
}
