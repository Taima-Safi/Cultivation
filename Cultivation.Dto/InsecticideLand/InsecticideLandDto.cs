using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Insecticide;

namespace Cultivation.Dto.InsecticideLand;

public class InsecticideLandDto
{
    public long Id { get; set; }
    //public string File { get; set; }
    public string Note { get; set; }
    public double Liter { get; set; }
    public DateTime Date { get; set; }
    public double? Quantity { get; set; } // for one liter

    public CuttingLandDto CuttingLand { get; set; }
    public InsecticideDto Insecticide { get; set; }
}
