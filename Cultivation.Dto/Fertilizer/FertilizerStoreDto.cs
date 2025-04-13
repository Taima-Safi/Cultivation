namespace Cultivation.Dto.Fertilizer;

public class FertilizerStoreDto
{
    public long Id { get; set; }
    public double TotalQuantity { get; set; }
    public FertilizerDto Fertilizer { get; set; }
}
