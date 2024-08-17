namespace Cultivation.Dto.CuttingLand;

public class CuttingLandFormDto
{
    public DateTime Date { get; set; }
    public long Quantity { get; set; }

    public long LandId { get; set; }
    public long CuttingColorId { get; set; }
}
