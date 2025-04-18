namespace Cultivation.Database.Model;

public class FertilizerApplicableMixModel : BaseModel
{
    public double DonumCount { get; set; }
    public double CurrentDonumCount { get; set; }

    public long FertilizerMixId { get; set; }
    public FertilizerMixModel FertilizerMix { get; set; }
}
