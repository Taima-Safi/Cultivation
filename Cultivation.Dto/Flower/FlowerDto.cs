using Cultivation.Dto.CuttingLand;

namespace Cultivation.Dto.Flower;

public class FlowerDto
{
    public long Id { get; set; }
    public string Note { get; set; }
    public int Count { get; set; }
    public double Long { get; set; }
    public DateTime Date { get; set; }

    public CuttingLandDto CuttingLand { get; set; }
}
