namespace Cultivation.Database.Model;

public class FertilizerMixDetailModel : BaseModel
{
    public double Quantity { get; set; } //kg

    public long FertilizerId { get; set; }
    public FertilizerModel Fertilizer { get; set; }

    public long FertilizerMixId { get; set; }
    public FertilizerMixModel FertilizerMix { get; set; }
}
