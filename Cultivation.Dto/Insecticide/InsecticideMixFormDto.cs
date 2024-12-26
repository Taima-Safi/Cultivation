using Cultivation.Dto.InsecticideLand;

namespace Cultivation.Dto.Insecticide;

public class InsecticideMixFormDto
{
    public string Note { get; set; }
    public string Title { get; set; }

    public List<InsecticideMixDto> Mixes { get; set; } = [];
}
