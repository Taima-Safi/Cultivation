namespace Cultivation.Dto.Fertilizer;

public class FertilizerApplicableMixDto
{
    public long Id { get; set; }
    public double DonumCount { get; set; }
    public double CurrentDonumCount { get; set; }

    public GetFertilizerMixDto FertilizerMixDto { get; set; }
}
