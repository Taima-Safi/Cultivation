namespace Cultivation.Dto.CuttingLand;

public class CuttingLandFormDto
{
    public DateTime Date { get; set; }

    public long LandId { get; set; }
    public List<CuttingFormDto> Cuttings { get; set; } = [];
}
