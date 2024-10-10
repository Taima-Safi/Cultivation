namespace Cultivation.Dto.InsecticideLand;

public class UpdateInsecticideLandFormDto
{
    public string Note { get; set; }
    public DateTime Date { get; set; }
    public double Liter { get; set; } //kg
    public double? Quantity { get; set; } //kg

    public long LandId { get; set; }
    public long InsecticideId { get; set; }
}
