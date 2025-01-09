using Cultivation.Dto.InsecticideLand;
using Cultivation.Shared.Enum;

namespace Cultivation.Dto.Insecticide;

public class InsecticideMixFormDto
{
    public string Note { get; set; }
    public string Title { get; set; }
    public ColorType Color { get; set; }

    public List<InsecticideMixDto> Mixes { get; set; } = [];
}
