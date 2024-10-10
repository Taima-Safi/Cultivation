namespace Cultivation.Dto.InsecticideLand;

public class InsecticideMixDto
{
    public double Liter { get; set; }
    public double? Quantity { get; set; } // if powder -> for one liter
    public long InsecticideId { get; set; }
}
