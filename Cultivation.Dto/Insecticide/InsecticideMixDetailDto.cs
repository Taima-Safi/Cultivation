namespace Cultivation.Dto.Insecticide;

public class InsecticideMixDetailDto
{
    public double Liter { get; set; }
    public double? Quantity { get; set; } //kg

    public InsecticideDto Insecticide { get; set; }
}
