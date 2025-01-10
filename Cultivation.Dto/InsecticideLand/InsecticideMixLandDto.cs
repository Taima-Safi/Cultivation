using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Insecticide;

namespace Cultivation.Dto.InsecticideLand;

public class InsecticideMixLandDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }

    public CuttingLandDto CuttingLand { get; set; }
    public GetInsecticideMixDto InsecticideMix { get; set; }
}
