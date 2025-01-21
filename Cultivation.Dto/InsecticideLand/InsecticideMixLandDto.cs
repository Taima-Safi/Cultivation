using Cultivation.Dto.Insecticide;
using Cultivation.Dto.Land;

namespace Cultivation.Dto.InsecticideLand;

public class InsecticideMixLandDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }

    public LandDto Land { get; set; }
    public GetInsecticideMixDto InsecticideMix { get; set; }
}
