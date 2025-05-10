namespace Cultivation.Dto.Insecticide;

public class InsecticideApplicableMixDto
{
    public long Id { get; set; }
    public double DonumCount { get; set; }
    public double CurrentDonumCount { get; set; }

    public GetInsecticideMixDto InsecticideMixDto { get; set; }
}
